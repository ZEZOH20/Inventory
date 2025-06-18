
import axios from 'axios'
import {useMutation,useQueryClient} from "@tanstack/react-query";

type UseDeleteProps = {
    url: string;
    qKey:  string;
}

const UseDeleteHandler = async (url:string) => {
    try{
        const response = await axios.delete(url);
        if(response.status == 200)
            return response.data;
    }catch(error: any){
        throw  {
            message: error.response.data,
        }
    }

}
const UseDelete = (props : UseDeleteProps) => {
    const queryClient = useQueryClient();
    const {url, qKey} = props;
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
        mutationFn:()=> UseDeleteHandler(url),
        onSuccess: () => {
            // Invalidate and refetch
            queryClient.invalidateQueries({ queryKey: [qKey] });
        },
    })
    return {data, isPending, error, isError, isSuccess , status,mutate};
}
export default  UseDelete;