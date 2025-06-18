import {Label, Select} from "flowbite-react";
import useGet from "../hooks/UseGet.tsx";


type  DropdownFormCProps = {
    data : {
        labelName: string;
        urlField:string;
        qKeyField:string;
        valueField : string;
        displayField:string;
    }
    
}
const DropdownFormC = (props: DropdownFormCProps) => {
    const {labelName,urlField,qKeyField,valueField,displayField} = props.data;
    const {
        data: mData,
        isLoading: mIsLoading,
        isError: mIsError
    } = useGet({
        url: urlField,
        qKey: qKeyField
    })
    if(mIsLoading){
        return <div className="bg-green-600">Loading...</div>
    }
    if(mIsError){
        return <div className="bg-red-600">Error</div>
    }
    return (<>
        <div className="max-w-md">
            <div className="mb-2 block">
                <Label htmlFor={`${labelName}`}> {labelName} </Label>
            </div>
            {/*onChange={(e) => onChange?.(e.target.value)}*/}
            <Select id={labelName} required >
                {
                    mData ? mData.map((item, index) => (
                        <option key={index} value={`${item[valueField]}`}>{item[displayField]}</option>
                    )) : <>
                    { mIsError ?? <div>There is an error can't fetch data</div> }
                    </>
                }
            </Select>
        </div>
    </>);
}
export default DropdownFormC;