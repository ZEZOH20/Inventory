import {Table, TableBody, TableCell, TableHead, TableRow, TableHeadCell, TextInput} from "flowbite-react";
import {BiSortAlt2} from "react-icons/bi";
import {GrNext, GrPrevious} from "react-icons/gr";
import useGet from "../hooks/UseGet.tsx";
import {
    createColumnHelper, flexRender,
    getCoreRowModel, getFilteredRowModel,
    getPaginationRowModel, getSortedRowModel,
    useReactTable
} from "@tanstack/react-table";
import {useContext, useEffect, useState} from "react";
import * as React from "react";
import {UpdateUser} from "../Components/Crud/Update/UpdateUser.tsx";
import {DeleteUser} from "../Components/Crud/Delete/DeleteUser.tsx";
import {globalContext} from "../App.tsx";


type Supplier = {
    id: number;
    name: string;
    phone: string;
    fax: string;
    mail: string;
    domain: string
}
const getRowId = (row:any,table:any)=>{
    // get primary key name
    const primaryKeyName = table.getAllColumns()[0]?.columnDef.accessorKey;
    return row.original[primaryKeyName];
}
const columnHelper = createColumnHelper<Supplier>();

const defaultColumns = [
    // Display Column
    columnHelper.accessor('id', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Id</span>,
        cell: info => <span>{info.getValue()}</span>
    }), columnHelper.accessor('name', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Name</span>,
        cell: info => <span>{info.getValue()}</span>,
        sortingFn: 'alphanumeric',
    }), columnHelper.accessor('phone', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Phone</span>,
        cell: info => <span>{info.getValue()}</span>,
        sortingFn: 'text',
    }), columnHelper.accessor('mail', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Mail</span>,
        cell: info => <span>{info.getValue()}</span>,
        sortingFn: 'alphanumeric',
    }), columnHelper.accessor('fax', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Fax</span>,
        cell: info => <span>{info.getValue()}</span>
    }), columnHelper.accessor('domain', {
        header: () => <span className={`flex items-center`}><BiSortAlt2/>Domain</span>,
        cell: info => <span>{info.getValue()}</span>
    }),
    columnHelper.display({
        id: 'update',
        cell: ({table, row}) => (
            <div className="flex gap-2"> {/* or use any styling you prefer */}
                <UpdateUser
                    tableName={'Supplier'}
                    getRowId={getRowId(row, table)}
                />
                <DeleteUser
                    tableName={'Supplier'}
                    getRowId={getRowId(row, table)}
                />
            </div>
        )
    }),

]
const Suppliers = () => {
    const {setGlobal} = useContext(globalContext)
    
    const [globalFilter, setGlobalFilter] = useState('');
    
    const [pagination, setPagination] = useState({
        pageIndex: 0,
        pageSize: 2,
    });
    
    const suppliersData = useGet({
        url: "https://localhost:7107/api/Supplier/getAll",
        qKey: "suppliers"
    });

    
    const table = useReactTable({
        data: suppliersData.data || [],
        columns: defaultColumns,
        
        getCoreRowModel: getCoreRowModel(),
        getSortedRowModel: getSortedRowModel(),
        getFilteredRowModel: getFilteredRowModel(),
        getPaginationRowModel: getPaginationRowModel(),
        
        state: {
            globalFilter,
            pagination,
        },
        
        onGlobalFilterChange: setGlobalFilter,
        onPaginationChange: setPagination,
    });
    useEffect(() => {
        setGlobal({
            // ...prev,
            loading: suppliersData.isLoading,
            errors:suppliersData.isError,
            errorMsg:suppliersData.error?.message,
        })
    },[suppliersData.isLoading,setGlobal])
    
    return (
        <TableComponent
            table={table}
            globalFilter={globalFilter}
            setGlobalFilter={setGlobalFilter}
            pagination={pagination}
        />
    );

}

type TableComponentProps = {
    table: any,
    globalFilter: string,
    setGlobalFilter: React.Dispatch<React.SetStateAction<string>>,
    pagination: { pageIndex: number, pageSize: number }
}
const TableComponent = (props: TableComponentProps) => {
    const {table, globalFilter, setGlobalFilter, pagination} = props;
    
    console.log(table.getRowModel().rows)
    return (
        <div className="overflow-x-auto">
            {/*search*/}
            <div className={`w-1/3`}>
                <TextInput id="search" value={globalFilter} type="text" placeholder="searching...." onChange={(e) =>setGlobalFilter(e.target.value)} />
            </div>
            {/*search*/}
            <Table>
                <TableHead>
                    {
                        table.getHeaderGroups().map((headerGroup: any) => (
                            <TableRow key={headerGroup.id}>
                                {
                                    headerGroup.headers.map((header: any) => (
                                        <TableHeadCell
                                            className={
                                                header.column.getCanSort()
                                                    ? 'cursor-pointer select-none'
                                                    : ''}
                                            key={header.id}
                                            onClick={header.column.getToggleSortingHandler()}
                                        >{
                                            flexRender(
                                                header.column.columnDef.header,
                                                header.getContext()
                                            )}
                                        </TableHeadCell>
                                    ))
                                }
                            </TableRow>
                        ))
                    }
                </TableHead>
                <TableBody>
                    {
                        table.getRowModel().rows.map((row: any) => {
                            return(
                                <TableRow key={row.id}>
                                    {
                                        row.getVisibleCells().map((cell: any) => (
                                            <TableCell key={cell.id}>{
                                                flexRender(
                                                    cell.column.columnDef.cell,
                                                    cell.getContext()
                                                )}
                                            </TableCell>
                                        ))
                                    }
                                </TableRow>
                            )
                            
                        })
                    }
                    {/*Pagination*/}
                    <TableRow>
                        <TableCell colSpan={table.getAllColumns().length} className={`  `}>
                            <div className="flex items-center justify-between">
                                <div>
                                    Showing 1 - {pagination.pageSize} of {" "}
                                    {table.getFilteredRowModel().rows.length}
                                </div>
                                <PaginationComponent table={table}/>
                            </div>
                        </TableCell>
                    </TableRow>
                    {/*Pagination*/}
                </TableBody>

            </Table>
        </div>
    )
}

const PaginationComponent = ({table}: { table: any }) => {
    // Get pagination state from table
    const {
        pageSize,
        pageIndex
    } = table.getState().pagination;

    const totalRows = table.getFilteredRowModel().rows.length;
    const totalPages = Math.ceil(totalRows / pageSize);
    const currentPage = pageIndex + 1;

    // Generate array of page numbers
    const pages = Array.from({length: totalPages}, (_, i) => i + 1);

    return (
        <div className="inline-flex items-stretch -space-x-px">
            <button
                onClick={() => table.previousPage()}
                disabled={!table.getCanPreviousPage()}
                className={`flex items-center justify-center h-full py-1.5 px-3 ml-0 text-gray-500 bg-white rounded-l-lg border border-gray-300 hover:bg-gray-100 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white`}
            >
                <GrPrevious/>
                Previous
            </button>
            {
                pages.map((page) => (
                    <button
                        key={page}
                        onClick={() => table.setPageIndex(page - 1)} // Subtract 1 since pageIndex is 0-based
                        className={`flex items-center justify-center text-sm py-1.5 px-3 leading-tight ${
                            currentPage === page
                                ? 'text-primary-600 bg-primary-50 border border-primary-300 hover:bg-primary-100 hover:text-primary-700 dark:border-gray-700 dark:bg-gray-700 dark:text-white'
                                : 'text-gray-500 bg-white border border-gray-300 hover:bg-gray-100 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white'
                        }`}
                    >
                        {page}
                    </button>
                ))
            }
            <button
                onClick={() => table.nextPage()}
                disabled={!table.getCanNextPage()}
                className={`flex items-center justify-center h-full py-1.5 px-3 ml-0 text-gray-500 bg-white rounded-r-lg border border-gray-300 hover:bg-gray-100 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white`}
            >Next
                <GrNext/>
            </button>
        </div>)
}
export default Suppliers;

