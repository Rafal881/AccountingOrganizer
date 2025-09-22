# Outbox Pattern in ClientOrganizer

## Core Principles Implemented

- **Transactional message storage** – Integration messages are stored in the same SQL Server database as domain data via the `OutboxMessages` table that is managed by Entity Framework. Saving a financial record and its outbound message now happens in the same transaction (see `FinanceService`).
- **Asynchronous dispatch** – A hosted background service (`OutboxProcessor`) polls pending rows and pushes them to Azure Service Bus outside of the request pipeline. API requests are now fast and shielded from temporary Service Bus outages.
- **Reliable delivery with retries** – Each message tracks retry attempts, the last error, and when it was processed. Failures are logged with incremental retry attempts up to a configurable limit to prevent message loss without flooding the bus.
- **Idempotence support** – Service Bus messages reuse the outbox message identifier as the `MessageId`. Combined with the persisted payload this enables downstream consumers to deduplicate deliveries.

## Why This Matters Here

- **Consistency between the API and the message bus** – Previously, calls to Azure Service Bus were executed inside HTTP requests. If sending failed after the database commit, the integration event was silently lost. The Outbox keeps the message until it can be delivered, guaranteeing that downstream systems (like the Function App that sends emails) observe every committed change.
- **Improved resilience** – External dependencies often fail transiently. Off-loading the send operation to the `OutboxProcessor` removes the need to surface infrastructure errors (e.g., Service Bus being temporarily unavailable) back to API clients.
- **Operational visibility** – Storing message metadata (timestamp, retry count, last error) gives operators insight into integration health and a place to apply diagnostics or manual remediation if a message exceeds the retry policy.
- **Scalability and flexibility** – The background dispatcher can be scaled, paused, or replaced without touching domain logic. Additional integration endpoints can reuse the same infrastructure simply by enqueuing new outbox messages.

