
import axios from 'axios'
import {useMutation,useQueryClient} from "@tanstack/react-query";


type UsePostProps = {
    url: string;
    requestData:object;
    qKey:  string;
    config?: object | undefined;
}

const UsePostHandler = async (url:string,data:object,config:object | undefined) => {
    try{
        const response = await axios.post(url, data, config);
        if(response.status == 200)
            return response.data;
    }catch(error: any){
        throw  {
            message: error.response.data,
        }
    }
    
}
const UsePost = (props : UsePostProps) => {
    const queryClient = useQueryClient();
    const {url,requestData, qKey, config} = props;
    const {
        data,
        error,
        isError,
        isPending,
        isSuccess,
        status,
        mutate
    } = useMutation({
        mutationKey: [qKey],
        mutationFn:()=> UsePostHandler(url,requestData,config),
        onSuccess: () => {
            // Invalidate and refetch
            queryClient.invalidateQueries({ queryKey: [qKey] });
        },
    })
    return {data, isPending, error, isError, isSuccess , status,mutate};
}
export default  UsePost;