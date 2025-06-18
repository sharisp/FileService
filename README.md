## 🔐 File Upload Service

Welcome! This is a file upload microservice built with **.NET 8**, following **Domain-Driven Design (DDD)** principles.

### 🌐 Supported Storage Providers

* **AWS S3**
* **Azure Blob Storage**


### 🧠 Smart Upload Deduplication

To **avoid redundant uploads**, the system checks if a file with the same **SHA256 hash** already exists.
If a match is found:

* The upload is **skipped**
* The existing file’s **URL is returned for reuse**

This improves **performance**, reduces **storage usage**, and enhances **user experience**.


### 🔒 Permission Control

This service requires permission validation **before uploading files**.
It integrates with my centralized permission management system:

👉 [Centralized Authority API](https://github.com/sharisp/Centralized.Authority)




Still updating...
