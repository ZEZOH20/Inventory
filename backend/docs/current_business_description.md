# Inventory Management System Business Summary

## Table of Contents

- [Introduction](#introduction)
- [Entities](#entities)
- [Relationships](#relationships)
- [Operations](#operations)
- [Rules](#rules)
- [Diagrams](#diagrams)
- [Version 2 Feature Suggestions](#version-2-feature-suggestions)

## Introduction

The Inventory Management System is designed to streamline the supply chain operations for a business dealing with products stored in multiple warehouses. The primary purpose is to manage the flow of goods from suppliers to warehouses, facilitate transfers between warehouses, and handle releases to customers. This system ensures efficient tracking of inventory levels, product details, and transactional records to support decision-making, maintain stock accuracy, and optimize logistics. Key workflows include procurement from suppliers, internal transfers for redistribution, and fulfillment of customer orders, all while adhering to constraints like product expiration dates and unit consistency.

## Entities

- **Person**: Represents individuals involved in the system, including basic contact information such as name, phone number, fax, email, and domain. This serves as a base for more specific roles.
- **User**: A type of person who acts as a warehouse manager, linked to a specific warehouse for oversight.
- **Supplier**: A type of person who provides products to the system, associated with supply orders and the products they deliver to warehouses.
- **Customer**: A type of person who receives products from warehouses, linked to release orders for order fulfillment.
- **Warehouse**: A storage facility identified by a unique number, with details like name, region, city, and street address. Each warehouse is managed by a user and contains various products.
- **Product**: An item in the inventory, defined by a unique code, name, and unit of measurement (e.g., kilogram, ton, pound, pack, dozen, liter, piece). Products are central to all inventory movements.
- **Warehouse_Product**: Represents the stock of a specific product in a warehouse, supplied by a supplier, including quantities, prices, manufacturing dates, expiration dates, and storage dates.
- **Supply_Order**: A record of products supplied by a supplier to a warehouse on a specific date, detailing the items and quantities involved.
- **Release_Order**: A record of products released from a warehouse to a customer on a specific date, outlining the items and quantities delivered.
- **Transfer_Order**: A record of products moved from one warehouse to another, facilitated through a supplier, on a specific date, including details of the items transferred.
- **SO_Product**: Details the specific products, amounts, units, prices, manufacturing dates, and expiration dates within a supply order.
- **RO_Product**: Details the specific products, amounts, units, and prices within a release order.
- **TO_Product**: Details the specific products, amounts, units, prices, manufacturing dates, and expiration dates within a transfer order.

## Relationships

- A User (as a manager) is associated with exactly one Warehouse.
- A Supplier is linked to multiple Warehouse_Products (the products they supply to warehouses) and has one associated Supply_Order.
- A Customer is linked to one Release_Order.
- A Warehouse is managed by one User, contains multiple Warehouse_Products, and is involved in Supply_Orders, Release_Orders, and Transfer_Orders (as source or destination).
- A Product is stored in multiple Warehouse_Products, and appears in multiple SO_Products, RO_Products, and TO_Products across orders.
- Warehouse_Product connects a Product, a Warehouse, and a Supplier, tracking inventory specifics.
- Supply_Order connects a Supplier and a Warehouse, and includes multiple SO_Products.
- Release_Order connects a Customer and a Warehouse, and includes multiple RO_Products.
- Transfer_Order connects a Supplier, a source Warehouse, and a destination Warehouse, and includes multiple TO_Products.
- SO_Product, RO_Product, and TO_Product link specific Products to their respective orders (Supply_Order, Release_Order, Transfer_Order).

## Operations

- **Supply Chain Procurement**: Suppliers create supply orders to deliver products to warehouses. This involves specifying the supplier, target warehouse, and details of products including amounts, units, prices, manufacturing dates, and expiration dates. Upon creation, the warehouse inventory is updated with the new stock.
- **Inventory Storage and Tracking**: Products are stored in warehouses with detailed records of quantities, total prices, and dates. Warehouse managers can update existing stock entries to reflect changes in amounts or other attributes.
- **Order Fulfillment for Customers**: Customers place release orders against warehouses, specifying products, amounts, units, and prices. This operation reduces warehouse inventory and records the transaction for customer delivery.
- **Internal Transfers**: Products can be transferred between warehouses via transfer orders, which involve a supplier intermediary. This includes specifying source and destination warehouses, products with amounts, units, prices, manufacturing dates, and expiration dates, updating inventory in both warehouses accordingly.
- **Query and Retrieval**: The system supports retrieving lists of all entities, such as supply orders, release orders, transfer orders, and warehouse inventories, to provide visibility into operations and stock levels.

## Rules

- All amounts and prices must be positive values greater than zero.
- Product units must conform to predefined categories (kilogram, ton, pound, pack, dozen, liter, piece).
- Manufacturing and expiration dates are mandatory for products in supply and transfer orders, and must be valid dates with expiration after manufacturing.
- Storage dates are recorded when products enter warehouse inventory.
- Each warehouse has a unique manager (user), and users are assigned to manage specific warehouses.
- Supply orders, release orders, and transfer orders must reference valid suppliers, customers, and warehouses.
- Inventory updates must ensure that releases and transfers do not exceed available stock in the source warehouse.
- Email addresses for persons must be valid and required.
- Phone numbers must follow a specific format (starting with 01 followed by specific digits).
- Transfers require a supplier association, even for internal movements between warehouses.

## Diagrams

### Supply Chain Flow

```
Supplier --> Supply Order --> Warehouse --> Release Order --> Customer
     |                           |
     +---------------------------+
             Transfer Order
```

This diagram illustrates the main flow of goods: from suppliers to warehouses via supply orders, storage in warehouses, releases to customers via release orders, and internal transfers between warehouses.

### Entity Relationships Overview

```
Person
├── User (manages) --> Warehouse
├── Supplier (supplies) --> Warehouse_Product --> Warehouse
└── Customer (receives) --> Release_Order --> Warehouse

Product --> Warehouse_Product
Product --> SO_Product --> Supply_Order
Product --> RO_Product --> Release_Order
Product --> TO_Product --> Transfer_Order
```

This shows the hierarchical relationships between entities, with Product as a central element connected to various order types and warehouse inventory.

## Version 2 Feature Suggestions

### Overview of Gaps and Limitations

Based on the current business description, the Inventory Management System is a foundational setup focused on core supply chain operations like procurement, storage, fulfillment, transfers, and basic queries. However, it has several key gaps and limitations that hinder scalability, efficiency, and user experience:

- **Single-Manager System**: Each warehouse is managed by exactly one user (warehouse manager), with no support for multiple roles, hierarchies, or administrative oversight. This limits flexibility in larger organizations where teams or admins need access.
- **Lack of Reports**: The system supports basic query and retrieval of lists (e.g., orders and inventories), but lacks advanced reporting, analytics, dashboards, or insights like stock trends, profitability, or performance metrics. Users must manually interpret raw data.
- **Basic Operations**: Core workflows (e.g., order creation, inventory updates) are manual and transactional without automation, notifications, approvals, or error prevention. For instance, no alerts for low stock, expiring products, or overstock; no workflow for order approvals; and no integration with external tools like accounting or e-commerce platforms.
- **Other Notable Gaps**: Limited user roles (only managers, suppliers, customers); no financial tracking beyond basic prices (e.g., no invoicing or payments); no demand forecasting or reordering suggestions; no audit trails or security features beyond basic validations; and no mobile/API access for remote usability.

These limitations make the system suitable for small-scale operations but inadequate for growing businesses needing enhanced control, insights, and automation. The following feature suggestions address these gaps, prioritizing appeal to warehouse managers (improved oversight and efficiency), suppliers (better communication and tracking), customers (enhanced order visibility), and admins (system-wide management). Each includes a pros/cons comparison between the current system and the upgraded version with the feature.

### Feature Suggestions with Pros/Cons Comparisons

#### 1. Advanced Reporting and Analytics Dashboard

**Description**: Implement a dashboard (web-based UI) displaying:

- **Supply Orders Section**: List/details of supply_orders with product info (amounts, units, prices, MFD/EXP) added to warehouses, filterable by date range (from/to). Includes totals per warehouse.
- **Release Orders Section**: List/details of release_orders with product info released from warehouses, filterable by date range. Includes totals per warehouse.
- **Transfer Orders Section**: List/details of transfer_orders with product info moved between warehouses, filterable by date range. Includes totals per warehouse.
- **Financial Summary Section**: Data on costs (e.g., total supply costs), revenues (e.g., total release revenues), and profit/loss (revenues minus costs), aggregated by date range or warehouse.

Users manually refresh data by triggering a server call (e.g., via a "Refresh" button). On-demand, extract selected data to PDF reports.

**Pros of Current System**:

- Straightforward data retrieval with basic lists, making it easy for users to access raw information without advanced tools.
- Minimal resource usage, as no heavy computations or storage for reports are needed.

**Cons of Current System**:

- Lack of insights leads to manual analysis, increasing time and errors (e.g., managers can't quickly spot trends or risks like stockouts).
- Poor usability for decision-making; users like warehouse managers and admins must use external tools for deeper analysis.
- Doesn't appeal to suppliers (no visibility into their performance) or customers (no order history summaries).

**Pros of Upgraded System**:

- Boosts usability with actionable insights, appealing to warehouse managers (real-time stock monitoring), suppliers (performance metrics on deliveries), customers (order tracking and history), and admins (system-wide KPIs).
- Enhances efficiency through automation (e.g., alerts for expiring products), reducing manual work and improving proactive management.
- Drives better decision-making with forecasting, potentially increasing sales and reducing waste.

**Cons of Upgraded System**:

- Higher computational and storage demands, which could slow performance in resource-constrained environments.
- Requires data integration and user training for report customization, potentially increasing costs and complexity.

#### 2. Automated Workflows and Notifications

**Description**: Enforce approvals for transactions:

- Employees create supply_orders, release_orders, or transfer_orders; managers approve/reject for their warehouses.
- Owners bypass approvals.
- Notifications: Basic email to manager when an order is submitted for approval (includes order details).
- Post-Approval: If approved, automatically add/update warehouse_products. If rejected, send email to employee with rejection reason and details.
- Audit: Automatic DB columns (CreatedBy, CreatedAt, ApprovedBy, ApprovedAt, Notes) in relevant tables.

**Pros of Current System**:

- Manual processes allow full control and customization per user, with no reliance on external services.
- Low risk of automation failures, keeping the system simple and reliable for basic tasks.

**Cons of Current System**:

- Time-consuming and error-prone (e.g., no alerts mean managers might miss low stock, leading to fulfillment issues).
- Poor usability for busy users; suppliers and customers lack real-time updates, and admins can't monitor without constant checks.
- Doesn't scale for high-volume operations, as everything is manual.

**Pros of Upgraded System**:

- Significantly enhances usability with proactive features, appealing to warehouse managers (automated alerts for inventory issues), suppliers (order status notifications), customers (delivery confirmations), and admins (system-wide monitoring).
- Reduces errors and improves efficiency through workflows (e.g., approvals prevent unauthorized actions), freeing users for strategic tasks.
- Increases responsiveness and user satisfaction, potentially improving retention and operational speed.

**Cons of Upgraded System**:

- Dependency on external services (e.g., email providers) could introduce reliability issues or security risks.
- Setup complexity for rules and notifications might require technical expertise, increasing maintenance and potential for misconfigurations.

#### 3. Multi-Role User Management System

**Description**: Simple roles with permissions:

- **Owner**: Full access (e.g., add/update warehouse_products without approvals, view all reports, approve any orders).
- **Manager**: Manages specific warehouses (approve orders/transfers for their warehouses only, view reports for their warehouses, create orders with self-approval if needed).
- **Employee**: Limited access (create supply_orders, release_orders, transfer_orders for submission; no direct warehouse_products updates; view basic reports).
- Notifications: Basic emails for pending approvals.
- Audit: Integrated with transaction audits (who performed actions).
- Admin capabilities: Owners/managers can view/query pending orders via UI/API for approval.

**Pros of Current System**:

- Simple and lightweight, with minimal setup for small teams (one manager per warehouse reduces complexity and potential conflicts).
- Low overhead for training and maintenance, as there's only one point of contact per warehouse.

**Cons of Current System**:

- Inflexible for scaling; large warehouses or multi-location businesses can't delegate tasks or have oversight without workarounds.
- No accountability or audit trails, increasing risks of errors or unauthorized changes.
- Limited appeal to admins or teams, as it doesn't support collaborative workflows.

**Pros of Upgraded System**:

- Enhances usability for warehouse managers (delegation of tasks), suppliers (dedicated admin roles for better coordination), customers (customer service reps for faster support), and admins (centralized control and monitoring).
- Improves security and compliance with RBAC, audit logs, and multi-user support, reducing errors and enabling better team collaboration.
- Appeals broadly by allowing role-specific dashboards (e.g., managers see inventory alerts, admins see system health).

**Cons of Upgraded System**:

- Increased complexity in setup and training, potentially raising initial costs and learning curves.
- Higher maintenance for permissions and user management, which could introduce configuration errors if not handled well.

#### 4. Financial and Payment Integration

**Description**: Expand beyond basic prices to include invoicing, payment tracking, and integration with accounting software (e.g., QuickBooks or SAP). Add features like cost analysis (e.g., per-unit costs, margins), payment gateways for customer orders, and supplier payment records.

**Pros of Current System**:

- Focuses on core inventory without financial overhead, keeping it lightweight for non-financial users.
- Avoids integration complexities, reducing setup time and potential errors.

**Cons of Current System**:

- Limited appeal to admins or financial stakeholders, as there's no end-to-end tracking of costs or revenues.
- Manual financial management outside the system leads to inefficiencies and data silos.

**Pros of Upgraded System**:

- Appeals to all users by providing financial visibility (e.g., warehouse managers see cost impacts, suppliers track payments, customers view invoices, admins monitor profitability).
- Enhances usability with streamlined processes, reducing manual accounting and improving accuracy.
- Supports business growth by enabling better budgeting and compliance.

**Cons of Upgraded System**:

- Requires integration with external financial tools, which could complicate deployments and increase costs.
- Potential security concerns with payment data, necessitating robust safeguards.
