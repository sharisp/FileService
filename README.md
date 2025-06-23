## 🔐 File Upload Service

Welcome! This is a **file upload microservice** built with **.NET 8**, designed using **Domain-Driven Design (DDD)** principles.

### 🌐 Supported Storage Providers

* **AWS S3**
* **Azure Blob Storage**

### 🧠 Smart Upload Deduplication

To prevent **redundant uploads**, the system calculates the **SHA256 hash** of each file. If a file with the same hash already exists:

* The upload is **skipped**
* The existing file’s **URL is returned for reuse**

This approach improves **performance**, reduces **storage usage**, and enhances the **user experience**.

### 📦Special Dependencies

This service uses the my 2 NuGet packages:

* [Domain.SharedKernel](https://github.com/sharisp/Domain.SharedKernel)
* [Common.Jwt](https://github.com/sharisp/Common.Jwt)

### 🔒 Permission Control

Before uploading, **permission validation** is required.

This is integrated with the centralized permission management system:

👉 [Centralized Authority API](https://github.com/sharisp/Centralized.Authority)

---
