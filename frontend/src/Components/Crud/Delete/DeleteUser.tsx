
"use client";

import useDelete from "../../../hooks/UseDelete.tsx";
import {Button} from "flowbite-react";
import {useContext, useEffect} from "react";
import {globalContext} from "../../../App.tsx";

type DeleteUserProps = {
    row?:any,
    table?:any,
    columns?:string[],
    getRowId:number,
    tableName:string,
}
export function DeleteUser(props:DeleteUserProps) {
    const {getRowId,tableName} = props;
    const user = useDelete({
        url: `https://localhost:7107/api/${tableName}/delete/${getRowId}`,
        qKey: "suppliers",
    });
    const {setGlobal} = useContext(globalContext)
    useEffect(() => {
        setGlobal({
            // ...prev,
            loading: user.isPending,
            errors:user.isError,
            errorMsg:user.error?.message,
        })
    },[user.isPending,setGlobal])
    
    return (
        <>
            <Button className={`dark:bg-red-600 dark:hover:bg-red-900`}  onClick={()=>user.mutate()}>Delete</Button>
        </>
    );
}
