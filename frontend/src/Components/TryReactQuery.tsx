import useGet from "../hooks/UseGet.tsx";
import usePost from "../hooks/UsePost.tsx";

const TryReactQuery = () => {
    const {data, isLoading, error, status} = useGet({
            url: "https://localhost:7107/api/Warehouse/getAll",
            qKey: "something"
        }
    );
    const {data: postData, mutate, isPending, error: postError, isError: postIsError} = usePost({
        url: "https://localhost:7107/api/Warehouse/create",
        requestData: {
            "name": "galal",
            "region": "string",
            "city": "string",
            "street": "string",
            "managerId": 1004
        },
        qKey: "anotherThing"
    })


    if (status === 'success') {
        console.log(data);
    } else {
        console.log(error?.message);
    }
    
    
    if(isPending) {
        console.log("isPending ...");
    }else{
        postIsError ?
            console.log(postError?.message) :
            console.log(postData)
    }
    
    
    return (<>
        {(isLoading || isPending) ? (
            <div className={`bg-red-600`}>isLoading ....</div>
        ) : (
            <div>
                <h1 className={`bg-blue-500`}>welcome to egypt</h1>
                <button onClick={()=>mutate()}>send something</button>
        
            </div>
        )}
    </>);
}
export default TryReactQuery;