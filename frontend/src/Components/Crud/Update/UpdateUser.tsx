"use client";

import {Button, HelperText, Label, Modal, ModalBody, ModalHeader, TextInput} from "flowbite-react";
import {useContext, useEffect, useState} from "react";
import usePut from "../../../hooks/UsePut.tsx";
import {useFormik} from "formik";
import {UpdateUserSchema} from "../../../shcemas/UpdateUserSchema.tsx";
import {globalContext} from "../../../App.tsx";




type UpdateUserProps = {
    // row?: any,
    // table?: any,
    // columns?: string[],
    getRowId: number,
    tableName: string,
   
}

export function UpdateUser(props: UpdateUserProps) {
    const {getRowId, tableName} = props;
    const {setGlobal} = useContext(globalContext)
    const [openModal, setOpenModal] = useState(false);
    
    const {values, handleChange, handleBlur, handleSubmit, errors, touched,resetForm} = useFormik({
        initialValues: {
            id: getRowId,
            name: "",
            phone: "",
        },
        validationSchema: UpdateUserSchema,
        onSubmit: (values)=>onSubmit(values),
    })

    const user = usePut({
        url: `https://localhost:7107/api/${tableName}/update`,
        qKey: "suppliers",
    });
    
    useEffect(() => {
        setGlobal({
            // ...prev,
            loading: user.isPending,
            errors:user.isError,
            errorMsg: user.error?.message || null
        })
    },[user.isPending,setGlobal])
    const onSubmit = async (values:any) => {
         try{
             await user.mutateAsync({
                 // id: getRowId,
                 ...values
             });
         }catch (e){
             console.log(e)
         }
        onCloseModal()
    }

    function onCloseModal() {
        setOpenModal(false);
        resetForm();
    }
    
    return (
        <>
            <Button onClick={() => setOpenModal(true)}>Update</Button>
            <Modal show={openModal} size="md" onClose={onCloseModal} popup>
                <ModalHeader/>
                <ModalBody>

                    {/*logic*/}
                    <form onSubmit={handleSubmit} className="grid max-w-md grid-cols-2 items-center gap-4">
                        <div>
                            {/*name*/}
                            <div className="mb-2 block">
                                <Label htmlFor="name" color={(errors.name && touched.name) &&`failure` }>
                                    name
                                </Label>
                            </div>
                            <TextInput
                                value={values.name}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                id="name"
                                placeholder={`${tableName} Name...`}
                                color={(errors.name && touched.name) && `failure`}

                            />
                            {errors.name && touched.name && <HelperText>
                                <span className="font-medium text-red-500">Oops!</span> {errors.name}!
                            </HelperText>}
                            
                            {/*name*/}
                            {/*phone*/}
                            <div className="mb-2 block">
                                <Label htmlFor="phone" color={(errors.phone && touched.phone) &&`failure`}>
                                    phone
                                </Label>
                            </div>
                            <TextInput
                                value={values.phone}
                                onChange={handleChange}
                                onBlur={handleBlur}
                                id="phone"
                                placeholder={`${tableName} Phone...`}
                                color={(errors.phone && touched.phone) && `failure` }

                            />
                            {errors.phone && touched.phone && <HelperText>
                                <span className="font-medium">Oops!</span> {errors.phone}!
                            </HelperText>}
                            
                            {/*phone*/}
                            
                        </div>

                        <Button type='submit'>
                            Update {tableName}
                        </Button>
                    </form>
                    {/*logic*/}
                </ModalBody>
            </Modal>


        </>
    );
}
