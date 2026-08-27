# Offline-First Architecture & Cloud Synchronization Design Document

## 1. Executive Summary
This document describes the architectural design required to evolve the local .NET MAUI POS application into a resilient, offline-first system capable of functioning uninterrupted during network outages and seamlessly synchronizing order data with a cloud backend when connectivity is restored.

---

## 2. Core Architectural Components

```
+-------------------------------------------------------------------+
|                        .NET MAUI Client App                       |
|                                                                   |
|   +-------------------+      +--------------------------------+   |
|   | UI & ViewModels   | ---> | Cart & Checkout Domain Logic   |   |
|   +-------------------+      +--------------------------------+   |
|                                             |                     |
|                                             v                     |
|                              +--------------------------------+   |
|                              | Local SQLite DB (Outbox Queue) |   |
|                              +--------------------------------+   |
|                                             |                     |
|                                             v                     |
|                              +--------------------------------+   |
|                              | Network & Sync Service Daemon  |   |
|                              +--------------------------------+   |
+---------------------------------------------|---------------------+
                                              | HTTPS / WebSockets
                                              v
                              +--------------------------------+
                              | Cloud API Gateway / Backend    |
                              +--------------------------------+
```

### **2.1 Transaction Outbox Pattern**
1. **Local Authoritative Writes:** All checkout transactions are immediately written to the local SQLite database (`Orders` table) with a `SyncState` column (`Pending`, `InFlight`, `Synced`, `Failed`).
2. **Outbox Queue Table (`SyncQueue`):** Every order generates an outbox payload item containing a deterministic UUID (`ClientOrderId`), timestamp, line item list, and idempotency token (`Idempotency-Key`).
3. **Zero UI Blocking:** The cashier receives instant order confirmation and receipt printing regardless of network state.

---

## 3. Synchronization & Connectivity Workflow

```
[Cashier Confirms Order]
        |
        v
[Save to SQLite with SyncState = Pending]
        |
        v
[Is Network Available?] 
   /               \
 (No)              (Yes)
  |                  |
[Remain Pending]   [Trigger Sync Daemon]
[Listen for        [POST /api/v1/orders/sync]
 Connectivity]       |
                     v
                [Server Returns 200 OK]
                     |
                [Update SyncState = Synced]
```

### **3.1 Connectivity Detection**
- Use .NET MAUI `Connectivity.Current.NetworkAccess`.
- Register an event listener for `Connectivity.ConnectivityChanged`.
- When connectivity switches to `NetworkAccess.Internet`, trigger an exponential backoff sync worker.

### **3.2 Idempotency & Conflict Resolution**
- **Idempotent API Requests:** Each sync HTTP POST request sends `X-Client-Order-Id: <UUID>` and `Idempotency-Key: <UUID>`.
- **Server Deduplication:** If the server received an order previously but the ACK network packet was lost, the backend returns the original `200 OK` response without duplicating the order in the cloud database.
- **Conflict Handling (Last-Write-Wins vs Client Priority):** Orders created offline are immutable financial records; client timestamps dictate exact checkout time, preventing backend override.

---

## 4. Security & Failure Recovery
- **Encrypted Local Storage:** Enable SQLCipher (`sqlite-net-sqlcipher`) for database encryption at rest on mobile/tablet devices.
- **JWT & Token Renewal:** Offline tokens expire gracefully; if token is expired upon reconnecting, sync daemon refreshes credentials prior to flushing the sync queue.
- **Dead Letter Queue (DLQ):** Orders failing validation (e.g. invalid server payload format) are flagged as `Failed` and written to a local DLQ table for manual audit by a manager without blocking subsequent orders.
