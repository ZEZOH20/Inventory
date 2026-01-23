# Inventory Management System API Endpoints Reference

## System Overview

The Inventory Management System supports role-based access with four user types:

- **Owner**: Full system administrator access
- **Manager**: Warehouse-specific management access
- **Customer**: External customer access for orders
- **Supplier**: External supplier access for supply orders

## Authentication Endpoints

### 1. User Login

**Endpoint**: `POST /api/auth/login`
**Who can use**: All users (unregistered users)
**What it does**: Authenticates user credentials and returns JWT token for API access

**Request Body**:

```json
{
  "email": "string",
  "password": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "string",
    "email": "string",
    "userName": "string",
    "role": "Owner|Manager|Customer|Supplier",
    "token": "jwt_token_string",
    "expiresOn": "2026-01-10T10:00:00Z"
  },
  "message": "Login successful",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User submits credentials → System validates → Returns token for subsequent requests

---

### 2. User Registration

**Endpoint**: `POST /api/auth/register`
**Who can use**: New users (unregistered)
**What it does**: Creates new user account with email verification

**Request Body**:

```json
{
  "userName": "string",
  "email": "string",
  "password": "string",
  "role": "Owner|Manager|Customer|Supplier",
  "userKey": "string",
  "otp": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "User registered successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User provides details + OTP → System validates → Account created → Can login

---

### 3. Send Verification Email

**Endpoint**: `POST /api/auth/send-verification-email`
**Who can use**: Users needing email verification
**What it does**: Sends OTP to email for account verification

**Request Body**:

```json
{
  "email": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "otp": "string",
    "userKey": "string"
  },
  "message": "Verification email sent",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests verification → System sends OTP → User receives OTP for registration

---

### 4. Reset Password

**Endpoint**: `POST /api/auth/reset-password`
**Who can use**: Registered users who forgot password
**What it does**: Resets user password using OTP verification

**Request Body**:

```json
{
  "email": "string",
  "otp": "string",
  "userKey": "string",
  "newPassword": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Password reset successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests reset → Receives OTP → Submits new password → Password updated

## User Management Endpoints

### 5. Get All Users

**Endpoint**: `GET /api/user/getAll`
**Who can use**: Owner only
**What it does**: Retrieves all system users

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "id": "integer",
      "name": "string",
      "mail": "string",
      "phone": "string",
      "warehouseId": "integer",
      "warehouseName": "string"
    }
  ],
  "message": "Users retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Owner requests → System returns all users → Owner can manage users

---

### 6. Create User

**Endpoint**: `POST /api/user/create`
**Who can use**: Owner only
**What it does**: Creates new user account

**Request Body**:

```json
{
  "name": "string",
  "mail": "string",
  "phone": "string",
  "password": "string",
  "warehouseId": "integer"
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "id": "integer",
    "name": "string",
    "mail": "string",
    "phone": "string"
  },
  "message": "User created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: Owner creates user → System validates → User account created

---

### 7. Update User

**Endpoint**: `PUT /api/user/update/{id}`
**Who can use**: Owner only
**What it does**: Updates existing user information

**Request Body**:

```json
{
  "name": "string",
  "mail": "string",
  "phone": "string",
  "warehouseId": "integer"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "integer",
    "name": "string",
    "mail": "string",
    "phone": "string"
  },
  "message": "User updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Owner modifies user → System updates → User information changed

---

### 8. Delete User

**Endpoint**: `DELETE /api/user/delete/{id}`
**Who can use**: Owner only
**What it does**: Soft deletes user account

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "User deleted successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Owner deletes user → System marks as deleted → User cannot access system

## Warehouse Management Endpoints

### 9. Get All Warehouses

**Endpoint**: `GET /api/warehouse/getAll`
**Who can use**: Owner, Manager
**What it does**: Retrieves warehouses (filtered by user permissions)

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "number": "integer",
      "name": "string",
      "region": "string",
      "city": "string",
      "street": "string",
      "manager": {
        "id": "string",
        "name": "string",
        "mail": "string"
      },
      "warehouse_Products": [
        {
          "product": {
            "id": "integer",
            "name": "string",
            "unit": "string"
          },
          "quantity": "decimal",
          "supplierName": "string"
        }
      ]
    }
  ],
  "message": "Warehouses retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests warehouses → System filters by permissions → Returns accessible warehouses

---

### 10. Create Warehouse

**Endpoint**: `POST /api/warehouse/create`
**Who can use**: Owner, Manager
**What it does**: Creates new warehouse

**Request Body**:

```json
{
  "name": "string",
  "region": "string",
  "city": "string",
  "street": "string",
  "managerId": "string"
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "number": "integer",
    "name": "string",
    "region": "string",
    "city": "string",
    "street": "string"
  },
  "message": "Warehouse created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: User creates warehouse → System validates → Warehouse added to system

---

### 11. Update Warehouse

**Endpoint**: `PUT /api/warehouse/update/{id}`
**Who can use**: Owner, Manager
**What it does**: Updates warehouse information

**Request Body**:

```json
{
  "name": "string",
  "region": "string",
  "city": "string",
  "street": "string",
  "managerId": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "number": "integer",
    "name": "string",
    "region": "string",
    "city": "string",
    "street": "string"
  },
  "message": "Warehouse updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User modifies warehouse → System updates → Warehouse information changed

---

### 12. Delete Warehouse

**Endpoint**: `DELETE /api/warehouse/delete/{id}`
**Who can use**: Owner, Manager
**What it does**: Soft deletes warehouse

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Warehouse deleted successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User deletes warehouse → System marks as deleted → Warehouse no longer available

## Product Management Endpoints

### 13. Get All Products

**Endpoint**: `GET /api/product/getAll`
**Who can use**: All authenticated users
**What it does**: Retrieves all products in system

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "id": "integer",
      "name": "string",
      "unit": "string",
      "description": "string"
    }
  ],
  "message": "Products retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests products → System returns all products → User can view product catalog

---

### 14. Create Product

**Endpoint**: `POST /api/product/create`
**Who can use**: Owner, Manager
**What it does**: Creates new product

**Request Body**:

```json
{
  "name": "string",
  "unit": "string",
  "description": "string"
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "id": "integer",
    "name": "string",
    "unit": "string",
    "description": "string"
  },
  "message": "Product created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: User creates product → System validates → Product added to catalog

---

### 15. Update Product

**Endpoint**: `PUT /api/product/update/{id}`
**Who can use**: Owner, Manager
**What it does**: Updates product information

**Request Body**:

```json
{
  "name": "string",
  "unit": "string",
  "description": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "integer",
    "name": "string",
    "unit": "string",
    "description": "string"
  },
  "message": "Product updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User modifies product → System updates → Product information changed

---

### 16. Delete Product

**Endpoint**: `DELETE /api/product/delete/{id}`
**Who can use**: Owner, Manager
**What it does**: Soft deletes product

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Product deleted successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User deletes product → System marks as deleted → Product no longer available

## Supply Order Management Endpoints

### 17. Get All Supply Orders

**Endpoint**: `GET /api/supplyorder/getAll`
**Who can use**: All authenticated users
**What it does**: Retrieves supply orders (filtered by user permissions)

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "number": "integer",
      "orderDate": "2026-01-10T10:00:00Z",
      "supplier": {
        "id": "integer",
        "name": "string"
      },
      "warehouse": {
        "number": "integer",
        "name": "string"
      },
      "status": "Pending|Approved|Rejected|Cancelled",
      "totalValue": "decimal",
      "so_Products": [
        {
          "product": {
            "id": "integer",
            "name": "string"
          },
          "so_Amount": "decimal",
          "so_Price": "decimal",
          "so_Mfd": "2026-01-10T10:00:00Z",
          "so_Exp": "2026-01-10T10:00:00Z"
        }
      ]
    }
  ],
  "message": "Supply orders retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests supply orders → System filters by permissions → Returns accessible orders

---

### 18. Create Supply Order

**Endpoint**: `POST /api/supplyorder/create`
**Who can use**: Owner, Manager
**What it does**: Creates new supply order for inventory replenishment

**Request Body**:

```json
{
  "supplierId": "integer",
  "war_Number": "integer",
  "so_Products": [
    {
      "productId": "integer",
      "so_Amount": "decimal",
      "so_Price": "decimal",
      "so_Mfd": "2026-01-10T10:00:00Z",
      "so_Exp": "2026-01-10T10:00:00Z"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "number": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Supply order created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: User creates supply order → Status set to Pending → Requires approval → Inventory updated when approved

---

### 19. Update Supply Order

**Endpoint**: `PUT /api/supplyorder/update/{id}`
**Who can use**: Owner, Manager
**What it does**: Updates supply order (only if pending)

**Request Body**:

```json
{
  "supplierId": "integer",
  "war_Number": "integer",
  "so_Products": [
    {
      "productId": "integer",
      "so_Amount": "decimal",
      "so_Price": "decimal",
      "so_Mfd": "2026-01-10T10:00:00Z",
      "so_Exp": "2026-01-10T10:00:00Z"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "number": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Supply order updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User modifies pending order → System updates → Order changes applied

---

### 20. Delete/Cancel Supply Order

**Endpoint**: `DELETE /api/supplyorder/delete/{id}`
**Who can use**: Owner, Manager
**What it does**: Cancels supply order

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Supply order cancelled successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User cancels order → Status changed to Cancelled → No inventory impact

## Release Order Management Endpoints

### 21. Get All Release Orders

**Endpoint**: `GET /api/releaseorder/getAll`
**Who can use**: All authenticated users
**What it does**: Retrieves release orders (filtered by user permissions)

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "number": "integer",
      "orderDate": "2026-01-10T10:00:00Z",
      "customer": {
        "id": "integer",
        "name": "string"
      },
      "warehouse": {
        "number": "integer",
        "name": "string"
      },
      "status": "Pending|Approved|Rejected|Cancelled",
      "totalValue": "decimal",
      "ro_Products": [
        {
          "product": {
            "id": "integer",
            "name": "string"
          },
          "ro_Amount": "decimal",
          "ro_Price": "decimal"
        }
      ]
    }
  ],
  "message": "Release orders retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests release orders → System filters by permissions → Returns accessible orders

---

### 22. Create Release Order

**Endpoint**: `POST /api/releaseorder/create`
**Who can use**: Owner, Manager, Customer
**What it does**: Creates new release order for product sales

**Request Body**:

```json
{
  "customerId": "integer",
  "war_Number": "integer",
  "ro_Products": [
    {
      "productId": "integer",
      "ro_Amount": "decimal",
      "ro_Price": "decimal"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "number": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Release order created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: User creates release order → Status set to Pending → Requires approval → Inventory reduced when approved

---

### 23. Update Release Order

**Endpoint**: `PUT /api/releaseorder/update/{id}`
**Who can use**: Owner, Manager, Customer (own orders)
**What it does**: Updates release order (only if pending)

**Request Body**:

```json
{
  "customerId": "integer",
  "war_Number": "integer",
  "ro_Products": [
    {
      "productId": "integer",
      "ro_Amount": "decimal",
      "ro_Price": "decimal"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "number": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Release order updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User modifies pending order → System updates → Order changes applied

---

### 24. Delete/Cancel Release Order

**Endpoint**: `DELETE /api/releaseorder/delete/{id}`
**Who can use**: Owner, Manager, Customer (own orders)
**What it does**: Cancels release order

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Release order cancelled successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User cancels order → Status changed to Cancelled → No inventory impact

## Transfer Order Management Endpoints

### 25. Get All Transfer Orders

**Endpoint**: `GET /api/transferorder/getAll`
**Who can use**: Owner, Manager
**What it does**: Retrieves transfer orders (filtered by user permissions)

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "id": "integer",
      "orderDate": "2026-01-10T10:00:00Z",
      "supplier": {
        "id": "integer",
        "name": "string"
      },
      "fromWarehouse": {
        "number": "integer",
        "name": "string"
      },
      "toWarehouse": {
        "number": "integer",
        "name": "string"
      },
      "status": "Pending|Approved|Rejected|Cancelled",
      "totalValue": "decimal",
      "to_Products": [
        {
          "product": {
            "id": "integer",
            "name": "string"
          },
          "to_Amount": "decimal",
          "to_Price": "decimal",
          "to_Mfd": "2026-01-10T10:00:00Z",
          "to_Exp": "2026-01-10T10:00:00Z"
        }
      ]
    }
  ],
  "message": "Transfer orders retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests transfer orders → System filters by permissions → Returns accessible orders

---

### 26. Create Transfer Order

**Endpoint**: `POST /api/transferorder/create`
**Who can use**: Owner, Manager
**What it does**: Creates transfer order between warehouses

**Request Body**:

```json
{
  "supplierId": "integer",
  "from": "integer",
  "to": "integer",
  "to_Products": [
    {
      "productId": "integer",
      "to_Amount": "decimal",
      "to_Price": "decimal",
      "to_Mfd": "2026-01-10T10:00:00Z",
      "to_Exp": "2026-01-10T10:00:00Z"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (201 Created):

```json
{
  "data": {
    "id": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Transfer order created successfully",
  "isSuccess": true,
  "statusCode": 201
}
```

**Flow**: User creates transfer → Status set to Pending → Requires approval → Inventory moves between warehouses

---

### 27. Update Transfer Order

**Endpoint**: `PUT /api/transferorder/update/{id}`
**Who can use**: Owner, Manager
**What it does**: Updates transfer order (only if pending)

**Request Body**:

```json
{
  "supplierId": "integer",
  "from": "integer",
  "to": "integer",
  "to_Products": [
    {
      "productId": "integer",
      "to_Amount": "decimal",
      "to_Price": "decimal",
      "to_Mfd": "2026-01-10T10:00:00Z",
      "to_Exp": "2026-01-10T10:00:00Z"
    }
  ]
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "integer",
    "orderDate": "2026-01-10T10:00:00Z",
    "status": "Pending"
  },
  "message": "Transfer order updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User modifies pending transfer → System updates → Transfer changes applied

---

### 28. Delete/Cancel Transfer Order

**Endpoint**: `DELETE /api/transferorder/delete/{id}`
**Who can use**: Owner, Manager
**What it does**: Cancels transfer order

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Transfer order cancelled successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User cancels transfer → Status changed to Cancelled → No inventory movement

## Approval Workflow Endpoints

### 29. Get Pending Orders

**Endpoint**: `GET /api/approval/pending`
**Who can use**: Owner, Manager
**What it does**: Retrieves orders pending approval

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "orderId": "integer",
      "orderNumber": "string",
      "orderType": "SupplyOrder|ReleaseOrder|TransferOrder",
      "submittedBy": "string",
      "warehouseName": "string",
      "status": "Pending",
      "totalValue": "decimal",
      "submittedAt": "2026-01-10T10:00:00Z"
    }
  ],
  "message": "Pending orders retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver requests pending orders → System returns orders needing approval → Approver can approve/reject

---

### 30. Approve Supply Order

**Endpoint**: `POST /api/approval/supply-orders/{orderId}/approve`
**Who can use**: Owner, Manager
**What it does**: Approves supply order and updates inventory

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Supply order approved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver approves → Status changes to Approved → Inventory increased → Order fulfilled

---

### 31. Reject Supply Order

**Endpoint**: `POST /api/approval/supply-orders/{orderId}/reject`
**Who can use**: Owner, Manager
**What it does**: Rejects supply order with notes

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Supply order rejected",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver rejects → Status changes to Rejected → No inventory impact → Order cancelled

---

### 32. Approve Release Order

**Endpoint**: `POST /api/approval/release-orders/{orderId}/approve`
**Who can use**: Owner, Manager
**What it does**: Approves release order and reduces inventory

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Release order approved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver approves → Status changes to Approved → Inventory decreased → Order fulfilled

---

### 33. Reject Release Order

**Endpoint**: `POST /api/approval/release-orders/{orderId}/reject`
**Who can use**: Owner, Manager
**What it does**: Rejects release order with notes

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Release order rejected",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver rejects → Status changes to Rejected → No inventory impact → Order cancelled

---

### 34. Approve Transfer Order

**Endpoint**: `POST /api/approval/transfer-orders/{orderId}/approve`
**Who can use**: Owner, Manager
**What it does**: Approves transfer and moves inventory between warehouses

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Transfer order approved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver approves → Status changes to Approved → Inventory moves from source to destination warehouse

---

### 35. Reject Transfer Order

**Endpoint**: `POST /api/approval/transfer-orders/{orderId}/reject`
**Who can use**: Owner, Manager
**What it does**: Rejects transfer order with notes

**Request Body**:

```json
{
  "reviewNotes": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Transfer order rejected",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: Approver rejects → Status changes to Rejected → No inventory movement → Transfer cancelled

## Reporting Endpoints

### 36. Supply Orders Report

**Endpoint**: `GET /api/reports/supply-orders`
**Who can use**: Owner, Manager
**What it does**: Generates supply orders report for date range

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "orderId": "integer",
      "orderNumber": "string",
      "orderDate": "2026-01-10T10:00:00Z",
      "supplierName": "string",
      "warehouseName": "string",
      "status": "string",
      "products": [
        {
          "productName": "string",
          "unit": "string",
          "quantity": "decimal",
          "unitPrice": "decimal",
          "totalPrice": "decimal",
          "manufacturingDate": "2026-01-10T10:00:00Z",
          "expirationDate": "2026-01-10T10:00:00Z"
        }
      ],
      "totalValue": "decimal"
    }
  ],
  "message": "Supply orders report generated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests report → System filters by date/warehouse → Returns formatted report data

---

### 37. Release Orders Report

**Endpoint**: `GET /api/reports/release-orders`
**Who can use**: Owner, Manager
**What it does**: Generates release orders report for date range

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "orderId": "integer",
      "orderNumber": "string",
      "orderDate": "2026-01-10T10:00:00Z",
      "customerName": "string",
      "warehouseName": "string",
      "status": "string",
      "products": [
        {
          "productName": "string",
          "unit": "string",
          "quantity": "decimal",
          "unitPrice": "decimal",
          "totalPrice": "decimal"
        }
      ],
      "totalValue": "decimal"
    }
  ],
  "message": "Release orders report generated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests report → System filters by date/warehouse → Returns formatted report data

---

### 38. Transfer Orders Report

**Endpoint**: `GET /api/reports/transfer-orders`
**Who can use**: Owner, Manager
**What it does**: Generates transfer orders report for date range

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

```json
{
  "data": [
    {
      "orderId": "integer",
      "orderNumber": "string",
      "orderDate": "2026-01-10T10:00:00Z",
      "supplierName": "string",
      "fromWarehouseName": "string",
      "toWarehouseName": "string",
      "status": "string",
      "products": [
        {
          "productName": "string",
          "unit": "string",
          "quantity": "decimal",
          "unitPrice": "decimal",
          "totalPrice": "decimal",
          "manufacturingDate": "2026-01-10T10:00:00Z",
          "expirationDate": "2026-01-10T10:00:00Z"
        }
      ],
      "totalValue": "decimal"
    }
  ],
  "message": "Transfer orders report generated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests report → System filters by date/warehouse → Returns formatted report data

---

### 39. Financial Summary Report

**Endpoint**: `GET /api/reports/financial-summary`
**Who can use**: Owner, Manager
**What it does**: Generates financial summary report for date range

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

```json
{
  "data": {
    "totalSupplyCosts": "decimal",
    "totalReleaseRevenues": "decimal",
    "totalTransferCosts": "decimal",
    "netProfitLoss": "decimal",
    "totalSupplyOrders": "integer",
    "totalReleaseOrders": "integer",
    "totalTransferOrders": "integer",
    "warehouseCosts": {
      "Warehouse A": "decimal",
      "Warehouse B": "decimal"
    },
    "warehouseRevenues": {
      "Warehouse A": "decimal",
      "Warehouse B": "decimal"
    }
  },
  "message": "Financial summary report generated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests financial report → System calculates metrics → Returns financial summary

---

### 40. Export Supply Orders PDF

**Endpoint**: `GET /api/reports/supply-orders/pdf`
**Who can use**: Owner, Manager
**What it does**: Exports supply orders report as PDF

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="supply-orders-report.pdf"`
- Body: PDF file bytes

**Flow**: User requests PDF → System generates formatted PDF → Returns downloadable file

---

### 41. Export Release Orders PDF

**Endpoint**: `GET /api/reports/release-orders/pdf`
**Who can use**: Owner, Manager
**What it does**: Exports release orders report as PDF

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="release-orders-report.pdf"`
- Body: PDF file bytes

**Flow**: User requests PDF → System generates formatted PDF → Returns downloadable file

---

### 42. Export Transfer Orders PDF

**Endpoint**: `GET /api/reports/transfer-orders/pdf`
**Who can use**: Owner, Manager
**What it does**: Exports transfer orders report as PDF

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="transfer-orders-report.pdf"`
- Body: PDF file bytes

**Flow**: User requests PDF → System generates formatted PDF → Returns downloadable file

---

### 43. Export Financial Summary PDF

**Endpoint**: `GET /api/reports/financial-summary/pdf`
**Who can use**: Owner, Manager
**What it does**: Exports financial summary report as PDF

**Request Body**: None

**Query Parameters**:

```json
{
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-01-31T23:59:59Z",
  "warehouseId": "integer (optional)"
}
```

**Expected Response** (200 OK):

- Content-Type: `application/pdf`
- Content-Disposition: `attachment; filename="financial-summary-report.pdf"`
- Body: PDF file bytes

**Flow**: User requests PDF → System generates formatted PDF → Returns downloadable file

## Profile Management Endpoints

### 44. Get Profile

**Endpoint**: `GET /api/profile`
**Who can use**: All authenticated users
**What it does**: Retrieves current user's profile information

**Request Body**: None

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "string",
    "name": "string",
    "email": "string",
    "phone": "string",
    "warehouseId": "integer",
    "warehouseName": "string",
    "profileImage": "string"
  },
  "message": "Profile retrieved successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User requests profile → System returns user data → User can view their information

---

### 45. Update Profile

**Endpoint**: `PUT /api/profile`
**Who can use**: All authenticated users
**What it does**: Updates current user's profile information

**Request Body**:

```json
{
  "name": "string",
  "phone": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "id": "string",
    "name": "string",
    "email": "string",
    "phone": "string"
  },
  "message": "Profile updated successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User updates profile → System validates → Profile information updated

---

### 46. Change Password

**Endpoint**: `POST /api/profile/change-password`
**Who can use**: All authenticated users
**What it does**: Changes current user's password

**Request Body**:

```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": null,
  "message": "Password changed successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User provides passwords → System validates current → Updates to new password

---

### 47. Upload Profile Image

**Endpoint**: `POST /api/profile/upload-image`
**Who can use**: All authenticated users
**What it does**: Uploads and updates user's profile image

**Request Body**: FormData with image file

**Query Parameters**: None

**Expected Response** (200 OK):

```json
{
  "data": {
    "imageUrl": "string"
  },
  "message": "Profile image uploaded successfully",
  "isSuccess": true,
  "statusCode": 200
}
```

**Flow**: User uploads image → System processes → Image saved and URL returned

## Error Handling

All endpoints follow consistent error response format:

```json
{
  "data": null,
  "message": "Error description",
  "isSuccess": false,
  "statusCode": 400
}
```

Common HTTP status codes:

- `400`: Validation errors or bad request
- `401`: Unauthorized (invalid/missing token)
- `403`: Forbidden (insufficient permissions)
- `404`: Resource not found
- `409`: Conflict (duplicate data)
- `500`: Internal server error

## System Flow Summary

1. **Authentication Flow**: Register → Verify Email → Login → Get Token → Access System
2. **Order Flow**: Create Order → Pending Status → Approval Required → Approved/Rejected → Inventory Update
3. **Inventory Flow**: Supply Orders increase stock, Release Orders decrease stock, Transfer Orders move stock
4. **Reporting Flow**: Request Report → Filter by Permissions → Generate Data → Return JSON/PDF
5. **Approval Flow**: Order Created → Appears in Pending → Approver Reviews → Approve/Reject → Status Updated

---

_This documentation provides endpoint-by-endpoint reference for the Inventory Management System API. Each endpoint includes authorization requirements, request/response formats, and operational flow._
