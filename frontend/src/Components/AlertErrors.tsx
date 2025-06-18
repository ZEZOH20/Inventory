"use client";
import { Modal, ModalBody, ModalHeader } from "flowbite-react";
import { useState } from "react";
import { HiOutlineExclamationCircle } from "react-icons/hi";

type AlertErrorsProps = {
    errors:Array<{ errorMessage: string; propertyName: string; }> | null,
}
const AlertErrors = ({errors} : AlertErrorsProps)=> {
    const [openModal, setOpenModal] = useState(true);

    return (
        <>
            <Modal show={openModal} size="md" onClose={() => setOpenModal(false)} popup >
                <ModalHeader className={`bg-red-200`} />
                <ModalBody className={`bg-red-200`}>
                    <div className="text-center ">
                        <HiOutlineExclamationCircle className="mx-auto mb-4 h-14 w-14 text-red-800 " />
                        {(!Array.isArray(errors)) && (
                            <p className="text-red-800">Server Error 500</p>
                        )}
                        {errors && Array.isArray(errors) && errors.map((err, idx)=>{
                            return (  <p key={idx} className="text-red-800">
                                {err.errorMessage}
                                {/*<strong>{err.propertyName}:</strong>*/}
                            </p>);
                        })}
                    </div>
                </ModalBody>
            </Modal>
        </>
    );
}
export default AlertErrors;