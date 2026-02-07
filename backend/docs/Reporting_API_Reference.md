# ReportingService API Reference

This document provides a comprehensive reference for the ReportingService endpoints, including input examples, parameter descriptions, response details, and conditions for no results. It serves as a guide for frontend developers to integrate with the inventory reporting system.

## Overview

The ReportingService exposes endpoints for generating reports on supply orders, release orders, transfer orders, and financial summaries. All endpoints require authentication and filter data based on user roles (Owner, Manager, Employee) and accessible warehouses.

## Endpoints

### 1. Supply Orders Report

- **Endpoint**: `GET /api/reports/supply-orders`
- **Example API Call**:
  ```
  GET /api/reports/supply-orders?startDate=2026-02-04&endDate=2026-02-06&warehouseId=26
  ```
- **Parameter Descriptions**:
  - `startDate` (string, required): The start date for filtering supply orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-04`). Must be a valid date and ≤ `endDate`.
  - `endDate` (string, required): The end date for filtering supply orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-06`).
  - `warehouseId` (integer, optional): ID of the warehouse to filter by (e.g., `26` for "MAinWareHouse"). If omitted, includes all warehouses accessible to the user (based on role: Owner sees their created warehouses, Manager sees their assigned one, Employee sees none).
- **When It Will Not Return Results**:
  - No supply orders exist in the date range for the user's accessible warehouses.
  - `startDate` > `endDate` or invalid date formats (e.g., `2026-13-01`).
  - `warehouseId` is provided but not accessible (e.g., Manager trying to access another warehouse) or doesn't exist.
  - User has no accessible warehouses (e.g., Employee role).
  - Database issues (e.g., soft-deleted warehouses/orders not filtered properly).
- **Response Parameter Descriptions** (for `Data` array of `SupplyOrderReportDto`):
  - `OrderId` (integer): Unique ID of the supply order (e.g., 62).
  - `OrderNumber` (string): Formatted order number (e.g., "SO-00062").
  - `OrderDate` (DateTime): Date the order was created (e.g., "2026-02-04T13:01:39").
  - `SupplierName` (string): Name of the supplier (e.g., "Adsaddas") or "Unknown" if null.
  - `WarehouseName` (string): Name of the destination warehouse (e.g., "MAinWareHouse") or "Unknown" if null.
  - `Status` (string): Order status as a string (e.g., "Approved" or "1" if enum).
  - `Products` (array of objects): List of products in the order.
    - `ProductName` (string): Name of the product (e.g., "a") or "Unknown".
    - `Unit` (string): Unit of measurement (e.g., "Piece").
    - `Quantity` (decimal): Quantity ordered (e.g., 1.00).
    - `UnitPrice` (decimal): Price per unit (e.g., 1.00).
    - `TotalPrice` (decimal): Total price for the product (Quantity \* UnitPrice, e.g., 1.00).
    - `ManufacturingDate` (DateTime?): Manufacturing date (e.g., "2026-02-04T00:00:00") or null.
    - `ExpirationDate` (DateTime?): Expiration date (e.g., "2026-02-26T00:00:00") or null.
  - `TotalValue` (decimal): Sum of all product `TotalPrice` in the order (e.g., 1.00).

### 2. Release Orders Report

- **Endpoint**: `GET /api/reports/release-orders`
- **Example API Call**:
  ```
  GET /api/reports/release-orders?startDate=2026-02-01&endDate=2026-02-07
  ```
- **Parameter Descriptions**:
  - `startDate` (string, required): The start date for filtering release orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-01`).
  - `endDate` (string, required): The end date for filtering release orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-07`).
  - `warehouseId` (integer, optional): ID of the warehouse to filter by. If omitted, includes all accessible warehouses.
- **When It Will Not Return Results**:
  - No release orders in the date range for accessible warehouses.
  - Invalid dates (e.g., `startDate` after `endDate`).
  - `warehouseId` not accessible or invalid.
  - No accessible warehouses for the user.
- **Response Parameter Descriptions** (for `Data` array of `ReleaseOrderReportDto`):
  - `OrderId` (integer): Unique ID of the release order.
  - `OrderNumber` (string): Formatted order number (e.g., "RO-00062").
  - `OrderDate` (DateTime): Date the order was created.
  - `CustomerName` (string): Name of the customer or "Unknown".
  - `WarehouseName` (string): Name of the source warehouse or "Unknown".
  - `Status` (string): Order status as a string.
  - `Products` (array of objects): List of products in the order (same structure as Supply Orders: ProductName, Unit, Quantity, UnitPrice, TotalPrice, ManufacturingDate, ExpirationDate).
  - `TotalValue` (decimal): Sum of all product `TotalPrice` in the order.

### 3. Transfer Orders Report

- **Endpoint**: `GET /api/reports/transfer-orders`
- **Example API Call**:
  ```
  GET /api/reports/transfer-orders?startDate=2026-02-05&endDate=2026-02-06&warehouseId=26
  ```
- **Parameter Descriptions**:
  - `startDate` (string, required): The start date for filtering transfer orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-05`).
  - `endDate` (string, required): The end date for filtering transfer orders (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-06`).
  - `warehouseId` (integer, optional): ID of the warehouse to filter by (includes transfers where this warehouse is the source or destination). If omitted, includes all accessible warehouses.
- **When It Will Not Return Results**:
  - No transfers in the date range involving accessible warehouses.
  - Invalid dates.
  - `warehouseId` not accessible (transfers must involve at least one accessible warehouse).
  - No accessible warehouses.
- **Response Parameter Descriptions** (for `Data` array of `TransferOrderReportDto`):
  - `OrderId` (integer): Unique ID of the transfer order.
  - `OrderNumber` (string): Formatted order number (e.g., "TO-00062").
  - `OrderDate` (DateTime): Date the order was created.
  - `SupplierName` (string): Name of the supplier (if applicable) or "Unknown".
  - `FromWarehouseName` (string): Name of the source warehouse or "Unknown".
  - `ToWarehouseName` (string): Name of the destination warehouse or "Unknown".
  - `Status` (string): Order status as a string.
  - `Products` (array of objects): List of products in the order (same structure as Supply Orders: ProductName, Unit, Quantity, UnitPrice, TotalPrice, ManufacturingDate, ExpirationDate).
  - `TotalValue` (decimal): Sum of all product `TotalPrice` in the order.

### 4. Financial Summary Report

- **Endpoint**: `GET /api/reports/financial-summary`
- **Example API Call**:
  ```
  GET /api/reports/financial-summary?startDate=2026-02-01&endDate=2026-02-07&warehouseId=26
  ```
- **Parameter Descriptions**:
  - `startDate` (string, required): The start date for aggregating financial data (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-01`).
  - `endDate` (string, required): The end date for aggregating financial data (inclusive). Format: `YYYY-MM-DD` (e.g., `2026-02-07`).
  - `warehouseId` (integer, optional): ID of the warehouse to filter financials by. If omitted, aggregates across all accessible warehouses.
- **When It Will Not Return Results**:
  - No orders/products in the date range for accessible warehouses (result will be empty or zero values).
  - Invalid dates.
  - `warehouseId` not accessible.
  - No accessible warehouses (summary will show zeros).
- **Response Parameter Descriptions** (for `Data` as `FinancialSummaryDto`):
  - `TotalSupplyCosts` (decimal): Total cost of all supply products in the period (sum of Quantity \* UnitPrice from SO_Products).
  - `TotalReleaseRevenues` (decimal): Total revenue from all release products (sum from RO_Product).
  - `TotalTransferCosts` (decimal): Total cost of transfers (sum from TO_Products).
  - `NetProfitLoss` (decimal): Calculated as TotalReleaseRevenues - (TotalSupplyCosts + TotalTransferCosts).
  - `TotalSupplyOrders` (integer): Count of supply orders in the period.
  - `TotalReleaseOrders` (integer): Count of release orders.
  - `TotalTransferOrders` (integer): Count of transfer orders.
  - `WarehouseCosts` (dictionary<string, decimal>): Breakdown of supply costs by warehouse name (e.g., {"MAinWareHouse": 100.00}).
  - `WarehouseRevenues` (dictionary<string, decimal>): Breakdown of release revenues by warehouse name.

## General Notes

- **Response Structure**: All responses include `IsSuccess` (bool), `Data` (the array/object above), and `Message` (string, e.g., "Report generated successfully").
- **Data Types**: Dates are in ISO format; decimals are currency values (assume USD).
- **Empty Responses**: If no data, `Data` is an empty array or object with zero/default values.
- **Validation Tips**: Always validate dates on the frontend (e.g., ensure `startDate` ≤ `endDate`). Use date pickers to prevent invalid formats. Check user role to show/hide `warehouseId` (e.g., hide for Employees).
- **Error Handling**: If no results, the API returns an empty array or zero values with a success message. Check for 400/500 errors on invalid inputs. Display user-friendly messages like "No data found for the selected period."
- **Performance**: For large date ranges, results may take time—consider loading indicators.
- **Testing**: Use the examples above with your sample data to verify. If issues occur, it might be due to data (e.g., no orders in range) or access (e.g., warehouse not created by owner).
