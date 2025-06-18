
import axios from "axios";
import {useQuery} from "@tanstack/react-query";
type useGetProps = {
    url: string,
    qKey:  string,
    config? :  object | undefined
}


    const getApiHandler  = async ( url: string, config: object | undefined )=> {
        try{
            const response = await axios.get(url, config)
            if(response.status == 200){
                return response.data
            }
            
        }catch(err : any){
            throw  {
                message: err.response.data,
            }
        }

}
const useGet = (props:useGetProps)=>{
    const {url , qKey, config } = props
    const {
        data,                // The fetched data
        isLoading,          // True during initial load
        isFetching,         // True during any fetch (including background)
        error,              // Error object if request failed
        isError,            // True if request failed
        refetch,            // Function to manually refetch
        status             // 'loading', 'error', or 'success'
    } = useQuery({
        queryKey:[qKey],
        queryFn :()=> getApiHandler(url, config),
    
        });
    
    
    return {data, isFetching, isLoading , error, isError, refetch , status};
}
export default useGet;