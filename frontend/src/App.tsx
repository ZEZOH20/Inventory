import SideNavbar from "./Components/SideNavbar.tsx";
import Navbar from "./Components/Navbar.tsx";
import {Routes, Route} from "react-router-dom";
import Warehouse from "./Pages/Warehouse.tsx";
import Managers from "./Pages/Managers.tsx";

import {QueryClient, QueryClientProvider} from "@tanstack/react-query"
import TryReactQuery from "./Components/TryReactQuery.tsx";
import {ToggleModalC} from "./Components/ToggleModalC.tsx";
import DropdownFormC from "./Components/DropdownFormC.tsx";
import ToggleModalFormC from "./Components/ToggleModalFormC.tsx";
import TableC from "./Components/TableC.tsx";
import Suppliers from "./Pages/Suppliers.tsx";
import {Spinner} from "flowbite-react";
import {createContext, useState} from "react";
import AlertErrors from "./Components/AlertErrors.tsx";
import NavbarC from "./Components/Navbar.tsx";
import SidebarC from "./Components/SideNavbar.tsx";
import {customTheme} from "./CustomTheme.tsx";
import {ThemeProvider} from 'flowbite-react';

type GlobalStateType = {
    loading: boolean;
    errors: boolean;
    errorMsg: Array<{ errorMessage: string; propertyName: string; }> | null;
};
export const globalContext = createContext({});
const queryClient = new QueryClient();

function App() {


    const [globalStates, setGlobalStates] = useState<GlobalStateType>({
        loading: false,
        errors: false,
        errorMsg: null,
    });
    const DropdownFormCData = {
        labelName: "Managers",
        urlField: "https://localhost:7107/api/User/getAll",
        qKeyField: "managers",
        valueField: "id",
        displayField: "name",
    }
    return (
        <>
            <div className="antialiased bg-gray-50 dark:bg-gray-900 h-screen overflow-y-hidden">

                <Routes>
                    {/*<Route path="/" element={<Navbar/>} />*/}
                    <Route path="/warehouse" element={<Warehouse/>}/>
                </Routes>

                {/*<Managers/>*/}
                {/*<Warehouse/>*/}
                <ThemeProvider theme={customTheme}>
                    <QueryClientProvider client={queryClient}>
                        <globalContext.Provider value={{getGlobal: globalStates, setGlobal: setGlobalStates}}>
                            {/*<TryReactQuery/>*/}
                            {/*<DropdownFormC data={DropdownFormCData}/>*/}
                            
                            {/*dashboard*/}
                            <div className={`grid grid-cols-6 `}>
                                <div className={` col-span-6 bg-red-500`}>
                                    <NavbarC/>
                                </div>
                                <div className={`col-span-1 bg-green-600`}>
                                    <SidebarC/>
                                </div>
                                <div className={`col-span-5 h-screen mt-20 `}>
                                    <div className={`w-4/5 mx-auto`}>
                                        <Suppliers/>
                                    </div>
                                </div>
                            </div>
                            {/*dashboard*/}

                            {/*<ToggleModalFormC/>*/}
                            {/*<TableC tableQKey={'managers'} tableUrl={`https://localhost:7107/api/User/getAll`}/>*/}
                        </globalContext.Provider>

                        {/*globals view*/}
                        {globalStates.loading &&
                            <Spinner className={` absolute top-1/2 left-1/2 translate-1/2`} aria-label="Loading..."/>
                        }
                        {globalStates.errors && <AlertErrors errors={globalStates.errorMsg}/>}
                        {/*globals view*/}

                    </QueryClientProvider>
                </ThemeProvider>


                {/*<main className="p-4 md:ml-64 h-auto pt-20">*/}
                {/*  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed border-gray-300 rounded-lg dark:border-gray-600 h-32 md:h-64"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-32 md:h-64"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-32 md:h-64"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-32 md:h-64"*/}
                {/*    ></div>*/}
                {/*  </div>*/}
                {/*  <div*/}
                {/*      className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-96 mb-4"*/}
                {/*  ></div>*/}
                {/*  <div className="grid grid-cols-2 gap-4 mb-4">*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*  </div>*/}
                {/*  <div*/}
                {/*      className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-96 mb-4"*/}
                {/*  ></div>*/}
                {/*  <div className="grid grid-cols-2 gap-4">*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*    <div*/}
                {/*        className="border-2 border-dashed rounded-lg border-gray-300 dark:border-gray-600 h-48 md:h-72"*/}
                {/*    ></div>*/}
                {/*  </div>*/}
                {/*</main>*/}
            </div>
        </>
    )
}

export default App
