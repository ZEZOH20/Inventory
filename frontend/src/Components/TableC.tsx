import {Table, TableBody, TableCell, TableHead, TableHeadCell, TableRow} from "flowbite-react";
import useGet from "../hooks/UseGet.tsx";

type  TableProps = {
    tableUrl: string;
    tableQKey: string;
}
const TableC = (props: TableProps) => {
    const {tableUrl, tableQKey} = props;
    const {
        data: tData,
        isLoading: tIsLoading,
        isError: tIsError
    } = useGet({
        url: tableUrl,
        qKey: tableQKey
    })
    if (tIsLoading) {
        return <div className="bg-green-600">Loading...</div>
    }
    if (tIsError) {
        return <div className="bg-red-600">Error</div>
    }
    return (<>

        <div className="overflow-x-auto">
            <Table>
                <TableHead>
                    <TableRow>
                        {tData ?? Object.keys(tData).map((hName) => (
                            <TableHeadCell>{hName}</TableHeadCell>
                        ))}
                        <TableHeadCell>
                            <span className="sr-only">Edit</span>
                        </TableHeadCell>
                    </TableRow>
                </TableHead>
                <TableBody className="divide-y">

                    {tData?.map((row, index) => (
                            <TableRow key={index} className="bg-white dark:border-gray-700 dark:bg-gray-800">
                                {Object.values(row).map((value, i) => (
                                    <TableCell key={i}>{JSON.stringify(value)}</TableCell>
                                ))}
                                <TableCell>
                                    <a href="#"
                                       className="font-medium text-primary-600 hover:underline dark:text-primary-500">
                                        Edit
                                    </a>
                                </TableCell>
                            </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>

    </>)
}
export default TableC