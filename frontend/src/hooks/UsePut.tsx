
import axios from 'axios'
import {useMutation,useQueryClient} from "@tanstack/react-query";


type UsePutProps = {
    url: string;
    qKey:  string;
    config?: object | undefined;
}

const UsePutHandler = async (url:string,data:object,config:object | undefined) => {
    try{
        const response = await axios.put(url, data, config);
        if(response.status == 200)
            return response.data;
    }catch(error: any){
        console.log('errorrrrrrrrrrrrrrrrrrrrrr' + JSON.stringify(error.response?.data[0]?.errorMessage));
        throw  {
            message: error.response.data ,
        }
        
    }

}
const UsePut = (props : UsePutProps) => {
    const queryClient = useQueryClient();
    const {url, qKey, config} = props;

    
    const {
        data,
        error,
        isError,
        isPending,
        isSuccess,
        status,
        mutate,
        mutateAsync
    } = useMutation({
        mutationKey: [qKey],
        mutationFn:(requestData: object)=> UsePutHandler(url,requestData,config),
        onSuccess: () => {
            // Invalidate and refetch
            queryClient.invalidateQueries({ queryKey: [qKey] });
        },
    })
    return {data, isPending, error, isError, isSuccess , status, mutate,mutateAsync};
}
export default  UsePut;