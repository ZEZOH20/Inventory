
import  * as yup from "yup";

export const UpdateUserSchema = yup.object().shape({
    name: yup.string().max(10 , "to much letters"),
    
})

    
