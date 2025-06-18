
import { Table, TableBody, TableCell, TableHead, TableHeadCell, TableRow } from "flowbite-react";
import {ToggleModalC} from "../Components/ToggleModalC.tsx";
import {DropdownC} from "../Components/DropdownC.tsx"
import {PopoverC} from "../Components/PopoverC.tsx"
const Managers = ()=>{
    return (<>
        <div className="overflow-x-auto">
            <Table>
                <TableHead>
                    <TableRow>
                        <TableHeadCell>Id</TableHeadCell>
                        <TableHeadCell>Name</TableHeadCell>
                        <TableHeadCell>Phone</TableHeadCell>
                        <TableHeadCell>Fax</TableHeadCell>
                        <TableHeadCell>Mail</TableHeadCell>
                        <TableHeadCell>Domain</TableHeadCell>
                        <TableHeadCell>
                            <span className="sr-only">Edit</span>
                        </TableHeadCell>
                    </TableRow>
                </TableHead>
                <TableBody className="divide-y">
                    <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                        <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                            Apple MacBook Pro 17"
                        </TableCell>
                        <TableCell>Sliver</TableCell>
                        <TableCell><PopoverC/></TableCell>
                        <TableCell><DropdownC/></TableCell>
                        <TableCell><ToggleModalC/></TableCell>
                        <TableCell>
                            <a href="#" className="font-medium text-cyan-600 hover:underline dark:text-cyan-500">
                                Edit
                            </a>
                        </TableCell>
                    </TableRow>
                    <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                        <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                            Apple MacBook Pro 17"
                        </TableCell>
                        <TableCell>Sliver</TableCell>
                        <TableCell>Laptop</TableCell>
                        <TableCell>$2999</TableCell>
                        <TableCell><ToggleModalC/></TableCell>
                        <TableCell>
                            <a href="#" className="font-medium text-cyan-600 hover:underline dark:text-cyan-500">
                                Edit
                            </a>
                        </TableCell>
                    </TableRow>
                    <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                        <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                            Apple MacBook Pro 17"
                        </TableCell>
                        <TableCell>Sliver</TableCell>
                        <TableCell>Laptop</TableCell>
                        <TableCell>$2999</TableCell>
                        <TableCell><ToggleModalC/></TableCell>
                        <TableCell>
                            <a href="#" className="font-medium text-cyan-600 hover:underline dark:text-cyan-500">
                                Edit
                            </a>
                        </TableCell>
                    </TableRow>
                </TableBody>
            </Table>
        </div>
    </>);
}

export default Managers;