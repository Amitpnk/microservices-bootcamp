# microservices-bootcamp (Chapter-wise)

## Part 1: Foundations of Microservices

| Module | Topic                        | Learning Objectives                                                                                                                                                                                                                                                                         |
|--------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1.1    | Introduction to Microservices | - What are Microservices?<br>- Monolith vs Microservices<br>- Benefits and Challenges                                                                                                                                                                                                             |
| 1.2    | Core Principles of Microservices | - Single Responsibility Principle<br>- SOLID and Design Principles<br>- Decentralization<br>- Scalability and Autonomy<br>- API Contracts                                                                                                                                                        |
| 1.3    | Designing Microservices       | - Bounded Context<br>- Domain-Driven Design (DDD)<br>- Service Granularity<br>- Designing APIs (REST, gRPC, GraphQL)                                                                                                                                                                               |
| 1.4    | Communication Patterns        | - Synchronous (REST/gRPC/graphQL)<br>- Asynchronous (message queues, events)                                                                                                                                                                                                  |
| 1.5    | Service Contracts             | - API-first Approach<br>- OpenAPI/Swagger/Scalar<br>- Backward Compatibility                                                                                                                                                                                                                     |
| 1.6    | Setting up Development Environment | - Tools & IDEs<br>- Docker and Containers<br>- Git Repositories<br>- Branching Strategy                                                                                                                                                              |

---

## Part 2: Building Microservices

| Module | Topic                | Learning Objectives                                                                                                                                                                                                                   |
|--------|----------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 2.1    | Tech Stack           | - Overview of Tech Choices<br>- .NET Core, Spring Boot, Node.js, etc.                                                                                                                                                                    |
| 2.2    | API Development      | - REST/gRPC APIs<br>- Build APIs with ASP.NET Core                                                                                                                                                                                       |
| 2.3    | Data Access Layer    | - EF Core<br>- CRUD Operations                                                                                                                                                                                                           |
| 2.4    | Event-Driven Architecture | - Event Sourcing<br>- Messaging (RabbitMQ, Kafka, Azure Service Bus)<br>- Publishing/Consuming Events                                                                                                                                |
| 2.5    | Data Management      | - Database-per-service<br>- CQRS<br>- Eventual Consistency                                                                                                                                                                                 |
| 2.6    | Service Discovery    | - Eureka, Consul, K8s DNS<br>- Registering and Discovering Services                                                                                                                               |
| 2.7    | Configuration Management | - AppSettings, YAML, Consul<br>- Centralized Configuration<br>- Azure App Configuration                                                                                                         |
| 2.8    | Testing Microservices | - Unit, Integration, Contract Testing<br>- Pact<br>- TestContainers                                                                                                                                |

---

## Part 3: Cross-Cutting Concerns & Patterns

| Module | Topic                        | Learning Objectives                                                                                                                                                 |
|--------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 3.1    | Resilience & Fault Tolerance | - Circuit Breaker<br>- Retry<br>- Timeout<br>- Polly<br>- Dapr                                                                                                               |
| 3.2    | API Gateway Pattern          | - API Gateway<br>- YARP<br>- Ocelot<br>- Azure API Management                                                                                                              |
| 3.3    | Security                     | - OAuth2<br>- OpenID Connect<br>- Identity Server<br>- Auth0<br>- Role/Policy-based Access                                                                                   |
| 3.4    | Logging & Monitoring         | - Centralized Logging<br>- Observability<br>- Serilog<br>- Seq<br>- ELK<br>- OpenTelemetry                                                                                     |
| 3.5    | Caching                      | - Distributed Caching<br>- Redis<br>- Cache Invalidation Strategies                                                                                                      |
| 3.6    | Service Mesh (Optional)      | - Istio<br>- Linkerd<br>- Consul Connect<br>- Introduction                                                                                                                 |

---

## Part 4: DevOps & Deployment

| Module | Topic                        | Learning Objectives                                                                                                                                                 |
|--------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 4.1    | Dockerizing                  | - Dockerfiles<br>- Run containers locally                                                                                                                               |
| 4.2    | Docker Compose               | - Orchestration<br>- Multiple microservices for local development                                                                                                       |
| 4.3    | Kubernetes Basics            | - AKS, GKE, EKS<br>- Deploy microservices                                                                                                                               |
| 4.4    | CI/CD Pipelines              | - GitHub Actions<br>- Azure DevOps<br>- Build → Test → Deploy<br>- Canary, Blue-Green Deployment                                                                            |
| 4.5    | Secrets & Config             | - ConfigMaps<br>- Secrets<br>- Key Vault<br>- Secure configuration in Kubernetes                                                                                            |
| 4.6    | Monitoring & Observability   | - Prometheus<br>- Grafana<br>- Fluentd<br>- Loki<br>- Metrics<br>- Health Checks<br>- Alerts<br>- Logging                                                                           |

---

## Part 5: Advanced Topics & Case Study

| Module | Topic                        | Learning Objectives                                                                                                                                                 |
|--------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 5.1    | Securing Microservices       | - API Security Best Practices<br>- Rate Limiting<br>- Secrets Management (Vault, Azure Key Vault)                                                                         |
| 5.2    | Anti-Patterns                | - Common Mistakes<br>- Avoid Tight Coupling<br>- Saga Pattern<br>- Other Design Patterns                                                                                   |
| 5.3    | AI/ML in Microservices       | - AI-Powered Microservices<br>- Model Serving and Integration                                                                                                           |
| 5.4    | Real-World Case Study        | - End-to-End Implementation<br>- Design and implement a microservice-based application                                                                                  |
| 5.5    | Interview Prep & Review      | - System Design Patterns<br>- Architecture reviews                                                                                                                      |
| 5.6    | Final Thoughts & Trends      | - Serverless<br>- Multi-cloud<br>- Azure Functions<br>- Multi-cloud Deployments                                                                                            |
