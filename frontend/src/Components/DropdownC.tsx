
import { Dropdown , DropdownItem } from "flowbite-react";

export function DropdownC() {
    return (
        <Dropdown label="Dropdown" inline>
            <DropdownItem>Dashboard</DropdownItem>
            <DropdownItem>Settings</DropdownItem>
            <DropdownItem>Earnings</DropdownItem>
            <DropdownItem>Sign out</DropdownItem>
        </Dropdown>
    );
}
