
import {createTheme} from "flowbite-react/helpers/create-theme";

export const customTheme = createTheme({
    // Color palette inspired by your design
    colors: {
        primary: {
            50: '#f0f9ff',
            100: '#e0f2fe',
            200: '#bae6fd',
            300: '#7dd3fc',
            400: '#38bdf8',
            500: '#0ea5e9',
            600: '#0284c7',
            700: '#0369a1',
            800: '#075985',
            900: '#0c4a6e',
        },
        gray: {
            50: '#f9fafb',
            100: '#f3f4f6',
            200: '#e5e7eb',
            300: '#d1d5db',
            400: '#9ca3af',
            500: '#6b7280',
            600: '#4b5563',
            700: '#374151',
            800: '#1f2937',
            900: '#111827',
        },
        success: {
            50: '#f0fdf4',
            100: '#dcfce7',
            200: '#bbf7d0',
            300: '#86efac',
            400: '#4ade80',
            500: '#22c55e',
            600: '#16a34a',
            700: '#15803d',
            800: '#166534',
            900: '#14532d',
        },
        warning: {
            50: '#fffbeb',
            100: '#fef3c7',
            200: '#fde68a',
            300: '#fcd34d',
            400: '#fbbf24',
            500: '#f59e0b',
            600: '#d97706',
            700: '#b45309',
            800: '#92400e',
            900: '#78350f',
        },
        danger: {
            50: '#fef2f2',
            100: '#fee2e2',
            200: '#fecaca',
            300: '#fca5a5',
            400: '#f87171',
            500: '#ef4444',
            600: '#dc2626',
            700: '#b91c1c',
            800: '#991b1b',
            900: '#7f1d1d',
        }
    },

    // Component themes
    components: {
        // Button component theme
        button: {
            base: "group relative flex items-stretch justify-center p-0.5 text-center font-medium transition-[color,background-color,border-color,text-decoration-color,fill,stroke,box-shadow] focus:z-10 focus:outline-none",
            fullSized: "w-full",
            color: {
                primary: "text-white bg-blue-600 border border-transparent hover:bg-blue-700 focus:ring-4 focus:ring-blue-300 disabled:hover:bg-blue-600 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800 dark:disabled:hover:bg-blue-600",
                secondary: "text-gray-900 bg-white border border-gray-200 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:border-gray-600 dark:hover:text-white dark:hover:bg-gray-700",
                success: "text-white bg-green-600 border border-transparent hover:bg-green-700 focus:ring-4 focus:ring-green-300 disabled:hover:bg-green-600 dark:bg-green-600 dark:hover:bg-green-700 dark:focus:ring-green-800 dark:disabled:hover:bg-green-600",
                danger: "text-white bg-red-600 border border-transparent hover:bg-red-700 focus:ring-4 focus:ring-red-300 disabled:hover:bg-red-600 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900 dark:disabled:hover:bg-red-600",
                warning: "text-white bg-yellow-400 border border-transparent hover:bg-yellow-500 focus:ring-4 focus:ring-yellow-300 disabled:hover:bg-yellow-400 dark:focus:ring-yellow-900",
                light: "text-gray-900 bg-white border border-gray-200 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:border-gray-600 dark:hover:text-white dark:hover:bg-gray-700",
            },
            size: {
                xs: "text-xs px-2 py-1",
                sm: "text-sm px-3 py-2",
                base: "text-sm px-5 py-2.5",
                lg: "text-base px-5 py-3",
                xl: "text-base px-6 py-3.5",
            },
            pill: "rounded-full",
            outline: {
                color: {
                    primary: "border border-blue-600 text-blue-600 hover:text-white hover:bg-blue-600 focus:ring-4 focus:ring-blue-300 dark:border-blue-500 dark:text-blue-500 dark:hover:text-white dark:hover:bg-blue-500",
                    secondary: "border border-gray-600 text-gray-600 hover:text-white hover:bg-gray-600 focus:ring-4 focus:ring-gray-300 dark:border-gray-500 dark:text-gray-500 dark:hover:text-white dark:hover:bg-gray-500",
                }
            }
        },

        // Card component theme
        card: {
            root: {
                base: "flex rounded-lg border border-gray-200 bg-white shadow-md dark:border-gray-700 dark:bg-gray-800",
                children: "flex h-full flex-col justify-center gap-4 p-6",
                horizontal: {
                    off: "flex-col",
                    on: "flex-col md:max-w-xl md:flex-row"
                },
                href: "hover:bg-gray-100 dark:hover:bg-gray-700"
            },
            img: {
                base: "",
                horizontal: {
                    off: "rounded-t-lg",
                    on: "h-96 w-full rounded-t-lg object-cover md:h-auto md:w-48 md:rounded-none md:rounded-l-lg"
                }
            }
        },

        // Table component theme
        table: {
            root: {
                base: "w-full text-left text-sm text-gray-500 dark:text-gray-400",
                shadow: "absolute bg-white dark:bg-black w-full h-full top-0 left-0 rounded-lg drop-shadow-md -z-10",
                wrapper: "relative"
            },
            body: {
                base: "group/body",
                cell: {
                    base: "group-first/body:group-first/row:first:rounded-tl-lg group-first/body:group-first/row:last:rounded-tr-lg group-last/body:group-last/row:first:rounded-bl-lg group-last/body:group-last/row:last:rounded-br-lg px-6 py-4"
                }
            },
            head: {
                base: "group/head text-xs uppercase text-gray-700 dark:text-gray-400",
                cell: {
                    base: "group-first/head:first:rounded-tl-lg group-first/head:last:rounded-tr-lg bg-gray-50 dark:bg-gray-700 px-6 py-3"
                }
            },
            row: {
                base: "group/row",
                hovered: "hover:bg-gray-50 dark:hover:bg-gray-600",
                striped: "odd:bg-white even:bg-gray-50 odd:dark:bg-gray-800 even:dark:bg-gray-700"
            }
        },

        // Navbar component theme
        navbar: {
            root: {
                base: "bg-white border-gray-200 px-2 py-2.5 dark:border-gray-700 dark:bg-gray-800 sm:px-4",
                rounded: {
                    on: "rounded",
                    off: ""
                },
                bordered: {
                    on: "border",
                    off: ""
                },
                inner: {
                    base: "mx-auto flex flex-wrap items-center justify-between",
                    fluid: {
                        on: "",
                        off: "container"
                    }
                }
            },
            brand: {
                base: "flex items-center"
            },
            collapse: {
                base: "w-full md:block md:w-auto",
                list: "mt-4 flex flex-col p-4 md:mt-0 md:flex-row md:space-x-8 md:p-0 md:text-sm md:font-medium",
                hidden: {
                    on: "hidden",
                    off: ""
                }
            },
            link: {
                base: "block py-2 pr-4 pl-3 md:p-0",
                active: {
                    on: "bg-blue-700 text-white dark:text-white md:bg-transparent md:text-blue-700",
                    off: "border-b border-gray-100 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white md:border-0 md:hover:bg-transparent md:hover:text-blue-700 md:dark:hover:bg-transparent md:dark:hover:text-white"
                },
                disabled: {
                    on: "text-gray-400 hover:cursor-not-allowed dark:text-gray-600",
                    off: ""
                }
            },
            toggle: {
                base: "inline-flex items-center p-2 ml-3 text-sm text-gray-500 rounded-lg md:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200 dark:text-gray-400 dark:hover:bg-gray-700 dark:focus:ring-gray-600",
                icon: "w-6 h-6"
            }
        },

        // Sidebar component theme
        sidebar: {
            root: {
                base: "h-full",
                collapsed: {
                    on: "w-16",
                    off: "w-64"
                },
                inner: "h-full overflow-y-auto overflow-x-hidden rounded bg-gray-50 py-4 px-3 dark:bg-gray-800"
            },
            collapse: {
                button: "group flex w-full items-center rounded-lg p-2 text-base font-normal text-gray-900 transition duration-75 hover:bg-gray-100 dark:text-white dark:hover:bg-gray-700",
                icon: {
                    base: "h-6 w-6 text-gray-500 transition duration-75 group-hover:text-gray-900 dark:text-gray-400 dark:group-hover:text-white",
                    open: {
                        off: "",
                        on: "text-gray-900"
                    }
                },
                label: {
                    base: "ml-3 text-left whitespace-nowrap",
                    icon: {
                        base: "h-6 w-6 transition ease-in-out delay-0",
                        open: {
                            on: "rotate-180",
                            off: ""
                        }
                    }
                },
                list: "space-y-2 py-2"
            },
            cta: {
                base: "mt-6 rounded-lg bg-blue-50 p-4 dark:bg-blue-900",
                color: {
                    blue: "bg-cyan-50 dark:bg-cyan-900",
                    dark: "bg-dark-50 dark:bg-dark-900",
                    failure: "bg-red-50 dark:bg-red-900",
                    gray: "bg-alternative-50 dark:bg-alternative-900",
                    green: "bg-green-50 dark:bg-green-900",
                    light: "bg-light-50 dark:bg-light-900",
                    red: "bg-red-50 dark:bg-red-900",
                    purple: "bg-purple-50 dark:bg-purple-900",
                    success: "bg-green-50 dark:bg-green-900",
                    yellow: "bg-yellow-50 dark:bg-yellow-900",
                    warning: "bg-yellow-50 dark:bg-yellow-900"
                }
            },
            item: {
                base: "flex items-center justify-center rounded-lg p-2 text-base font-normal text-gray-900 hover:bg-gray-100 dark:text-white dark:hover:bg-gray-700",
                active: "bg-gray-100 dark:bg-gray-700",
                collapsed: {
                    insideCollapse: "group w-full pl-8 transition duration-75",
                    noIcon: "font-bold"
                },
                content: {
                    base: "px-3 flex-1 whitespace-nowrap"
                },
                icon: {
                    base: "h-6 w-6 flex-shrink-0 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white",
                    active: "text-gray-700 dark:text-gray-100"
                },
                label: "ml-3",
                listItem: ""
            },
            items: "space-y-2",
            itemGroup: "mt-4 space-y-2 border-t border-gray-200 pt-4 first:mt-0 first:border-t-0 first:pt-0 dark:border-gray-700",
            logo: {
                base: "mb-5 flex items-center pl-2.5",
                collapsed: {
                    on: "hidden",
                    off: "self-center whitespace-nowrap text-xl font-semibold dark:text-white"
                },
                img: "mr-3 h-6 sm:h-7"
            }
        },

        // Modal component theme
        modal: {
            root: {
                base: "fixed top-0 left-0 right-0 z-50 h-[calc(100%-1rem)] max-h-full w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0",
                show: {
                    on: "flex bg-gray-900 bg-opacity-50 dark:bg-opacity-80",
                    off: "hidden"
                },
                sizes: {
                    sm: "max-w-md",
                    md: "max-w-lg",
                    lg: "max-w-4xl",
                    xl: "max-w-7xl"
                }
            },
            content: {
                base: "relative h-full w-full p-4 md:h-auto",
                inner: "relative rounded-lg bg-white shadow dark:bg-gray-700 flex flex-col max-h-[90vh]"
            },
            body: {
                base: "p-6 flex-1 overflow-auto",
                popup: "pt-0"
            },
            header: {
                base: "flex items-start justify-between p-4 border-b rounded-t dark:border-gray-600",
                popup: "p-2 border-b-0",
                title: "text-xl font-semibold text-gray-900 dark:text-white",
                close: {
                    base: "ml-auto inline-flex items-center rounded-lg bg-transparent p-1.5 text-sm text-gray-400 hover:bg-gray-200 hover:text-gray-900 dark:hover:bg-gray-600 dark:hover:text-white",
                    icon: "h-5 w-5"
                }
            },
            footer: {
                base: "flex items-center p-6 space-x-2 border-t border-gray-200 rounded-b dark:border-gray-600",
                popup: "border-t-0"
            }
        },

        // Input field theme
        textInput: {
            base: "flex",
            addon: "inline-flex items-center px-3 text-sm text-gray-900 bg-gray-200 border border-r-0 border-gray-300 rounded-l-md dark:bg-gray-600 dark:text-gray-400 dark:border-gray-600",
            field: {
                base: "relative w-full",
                icon: {
                    base: "pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3",
                    svg: "h-5 w-5 text-gray-500 dark:text-gray-400"
                },
                rightIcon: {
                    base: "pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3",
                    svg: "h-5 w-5 text-gray-500 dark:text-gray-400"
                },
                input: {
                    base: "block w-full border disabled:cursor-not-allowed disabled:opacity-50",
                    sizes: {
                        sm: "p-2 sm:text-xs",
                        md: "p-2.5 text-sm",
                        lg: "sm:text-md p-4"
                    },
                    colors: {
                        gray: "bg-gray-50 border-gray-300 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500",
                        info: "border-blue-500 bg-blue-50 text-blue-900 placeholder-blue-700 focus:border-blue-500 focus:ring-blue-500 dark:border-blue-400 dark:bg-blue-100 dark:focus:border-blue-500 dark:focus:ring-blue-500",
                        failure: "border-red-500 bg-red-50 text-red-900 placeholder-red-700 focus:border-red-500 focus:ring-red-500 dark:border-red-400 dark:bg-red-100 dark:focus:border-red-500 dark:focus:ring-red-500",
                        warning: "border-yellow-500 bg-yellow-50 text-yellow-900 placeholder-yellow-700 focus:border-yellow-500 focus:ring-yellow-500 dark:border-yellow-400 dark:bg-yellow-100 dark:focus:border-yellow-500 dark:focus:ring-yellow-500",
                        success: "border-green-500 bg-green-50 text-green-900 placeholder-green-700 focus:border-green-500 focus:ring-green-500 dark:border-green-400 dark:bg-green-100 dark:focus:border-green-500 dark:focus:ring-green-500"
                    },
                    withRightIcon: {
                        on: "pr-10",
                        off: ""
                    },
                    withIcon: {
                        on: "pl-10",
                        off: ""
                    },
                    withAddon: {
                        on: "rounded-r-lg",
                        off: "rounded-lg"
                    },
                    withShadow: {
                        on: "shadow-sm dark:shadow-sm-light",
                        off: ""
                    }
                }
            }
        },

        // Select component theme
        select: {
            base: "flex",
            addon: "inline-flex items-center px-3 text-sm text-gray-900 bg-gray-200 border border-r-0 border-gray-300 rounded-l-md dark:bg-gray-600 dark:text-gray-400 dark:border-gray-600",
            field: {
                base: "relative w-full",
                icon: {
                    base: "pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3",
                    svg: "h-5 w-5 text-gray-500 dark:text-gray-400"
                },
                select: {
                    base: "block w-full border disabled:cursor-not-allowed disabled:opacity-50",
                    sizes: {
                        sm: "p-2 sm:text-xs",
                        md: "p-2.5 text-sm",
                        lg: "sm:text-md p-4"
                    },
                    colors: {
                        gray: "bg-gray-50 border-gray-300 text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500",
                        info: "border-blue-500 bg-blue-50 text-blue-900 placeholder-blue-700 focus:border-blue-500 focus:ring-blue-500 dark:border-blue-400 dark:bg-blue-100 dark:focus:border-blue-500 dark:focus:ring-blue-500",
                        failure: "border-red-500 bg-red-50 text-red-900 placeholder-red-700 focus:border-red-500 focus:ring-red-500 dark:border-red-400 dark:bg-red-100 dark:focus:border-red-500 dark:focus:ring-red-500",
                        warning: "border-yellow-500 bg-yellow-50 text-yellow-900 placeholder-yellow-700 focus:border-yellow-500 focus:ring-yellow-500 dark:border-yellow-400 dark:bg-yellow-100 dark:focus:border-yellow-500 dark:focus:ring-yellow-500",
                        success: "border-green-500 bg-green-50 text-green-900 placeholder-green-700 focus:border-green-500 focus:ring-green-500 dark:border-green-400 dark:bg-green-100 dark:focus:border-green-500 dark:focus:ring-green-500"
                    },
                    withIcon: {
                        on: "pl-10",
                        off: ""
                    },
                    withAddon: {
                        on: "rounded-r-lg",
                        off: "rounded-lg"
                    },
                    withShadow: {
                        on: "shadow-sm dark:shadow-sm-light",
                        off: ""
                    }
                }
            }
        },

        // Badge component theme
        badge: {
            root: {
                base: "flex h-fit items-center gap-1 font-semibold",
                color: {
                    primary: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300",
                    secondary: "bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300",
                    success: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300",
                    danger: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300",
                    warning: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300",
                    indigo: "bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-300",
                    purple: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300",
                    pink: "bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-300"
                },
                href: "group",
                size: {
                    xs: "p-1 text-xs",
                    sm: "p-1.5 text-xs",
                    base: "p-2 text-sm",
                    lg: "p-2.5 text-base",
                    xl: "p-3 text-base"
                }
            },
            icon: {
                off: "rounded-full",
                on: "rounded-full p-1",
                size: {
                    xs: "h-3 w-3",
                    sm: "h-3.5 w-3.5",
                    base: "h-4 w-4",
                    lg: "h-5 w-5",
                    xl: "h-6 w-6"
                }
            }
        },

        // Alert component theme
        alert: {
            base: "flex flex-col gap-2 p-4 text-sm",
            borderAccent: "border-t-4",
            closeButton: {
                base: "-mx-1.5 -my-1.5 ml-auto inline-flex h-8 w-8 rounded-lg p-1.5 focus:ring-2",
                icon: "h-5 w-5",
                color: {
                    info: "bg-blue-50 text-blue-500 hover:bg-blue-200 focus:ring-blue-400 dark:bg-blue-800 dark:text-blue-300 dark:hover:bg-blue-900",
                    gray: "bg-gray-50 text-gray-500 hover:bg-gray-200 focus:ring-gray-400 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-900",
                    failure: "bg-red-50 text-red-500 hover:bg-red-200 focus:ring-red-400 dark:bg-red-800 dark:text-red-300 dark:hover:bg-red-900",
                    success: "bg-green-50 text-green-500 hover:bg-green-200 focus:ring-green-400 dark:bg-green-800 dark:text-green-300 dark:hover:bg-green-900",
                    warning: "bg-yellow-50 text-yellow-500 hover:bg-yellow-200 focus:ring-yellow-400 dark:bg-yellow-800 dark:text-yellow-300 dark:hover:bg-yellow-900",
                    red: "bg-red-50 text-red-500 hover:bg-red-200 focus:ring-red-400 dark:bg-red-800 dark:text-red-300 dark:hover:bg-red-900",
                    green: "bg-green-50 text-green-500 hover:bg-green-200 focus:ring-green-400 dark:bg-green-800 dark:text-green-300 dark:hover:bg-green-900",
                    blue: "bg-blue-50 text-blue-500 hover:bg-blue-200 focus:ring-blue-400 dark:bg-blue-800 dark:text-blue-300 dark:hover:bg-blue-900"
                }
            },
            color: {
                info: "border-blue-500 bg-blue-50 text-blue-800 dark:bg-blue-800 dark:text-blue-400",
                gray: "border-gray-500 bg-gray-50 text-gray-800 dark:bg-gray-800 dark:text-gray-400",
                failure: "border-red-500 bg-red-50 text-red-800 dark:bg-red-800 dark:text-red-400",
                success: "border-green-500 bg-green-50 text-green-800 dark:bg-green-800 dark:text-green-400",
                warning: "border-yellow-500 bg-yellow-50 text-yellow-800 dark:bg-yellow-800 dark:text-yellow-400",
                red: "border-red-500 bg-red-50 text-red-800 dark:bg-red-800 dark:text-red-400",
                green: "border-green-500 bg-green-50 text-green-800 dark:bg-green-800 dark:text-green-400",
                blue: "border-blue-500 bg-blue-50 text-blue-800 dark:bg-blue-800 dark:text-blue-400"
            },
            icon: "mr-3 inline h-5 w-5 flex-shrink-0",
            rounded: "rounded-lg",
            wrapper: "flex items-center"
        }
    },

    // Typography settings
    typography: {
        fontFamily: {
            sans: ['Inter', 'ui-sans-serif', 'system-ui', 'sans-serif'],
            serif: ['ui-serif', 'serif'],
            mono: ['ui-monospace', 'monospace']
        },
        fontSize: {
            xs: ['0.75rem', { lineHeight: '1rem' }],
            sm: ['0.875rem', { lineHeight: '1.25rem' }],
            base: ['1rem', { lineHeight: '1.5rem' }],
            lg: ['1.125rem', { lineHeight: '1.75rem' }],
            xl: ['1.25rem', { lineHeight: '1.75rem' }],
            '2xl': ['1.5rem', { lineHeight: '2rem' }],
            '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
            '4xl': ['2.25rem', { lineHeight: '2.5rem' }],
            '5xl': ['3rem', { lineHeight: '1' }],
            '6xl': ['3.75rem', { lineHeight: '1' }]
        }
    },

    // Spacing and layout
    spacing: {
        containerPadding: {
            DEFAULT: '1rem',
            sm: '2rem',
            lg: '4rem',
            xl: '5rem',
            '2xl': '6rem',
        },
    },

    // Border radius
    borderRadius: {
        none: '0',
        sm: '0.125rem',
        DEFAULT: '0.25rem',
        md: '0.375rem',
        lg: '0.5rem',
        xl: '0.75rem',
        '2xl': '1rem',
        '3xl': '1.5rem',
        full: '9999px',
    },

    // Box shadows
    boxShadow: {
        sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
        DEFAULT: '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
        md: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
        xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
        '2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
        inner: 'inset 0 2px 4px 0 rgba(0, 0, 0, 0.06)',
    }
})

import {Button, Card, Table, Badge, TableCell, TableRow, TableBody, TableHeadCell, TableHead} from 'flowbite-react';

export function InventoryDashboard() {
    return (
        <div className="p-6 bg-gray-50">
            <div className="mb-6">
                <h1 className="text-3xl font-bold text-gray-900 mb-2">Inventory Management</h1>
                <p className="text-gray-600">Manage your products and track inventory levels</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                <Card>
                    <div className="flex items-center">
                        <div className="p-3 rounded-full bg-blue-100 mr-4">
                            <svg className="w-6 h-6 text-blue-600" fill="currentColor" viewBox="0 0 20 20">
                                <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"/>
                            </svg>
                        </div>
                        <div>
                            <p className="text-2xl font-bold text-gray-900">1,344</p>
                            <p className="text-sm text-gray-600">Total Products</p>
                        </div>
                    </div>
                </Card>

                <Card>
                    <div className="flex items-center">
                        <div className="p-3 rounded-full bg-green-100 mr-4">
                            <svg className="w-6 h-6 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd"/>
                            </svg>
                        </div>
                        <div>
                            <p className="text-2xl font-bold text-gray-900">19</p>
                            <p className="text-sm text-gray-600">Active Orders</p>
                        </div>
                    </div>
                </Card>

                <Card>
                    <div className="flex items-center">
                        <div className="p-3 rounded-full bg-yellow-100 mr-4">
                            <svg className="w-6 h-6 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd"/>
                            </svg>
                        </div>
                        <div>
                            <p className="text-2xl font-bold text-gray-900">200</p>
                            <p className="text-sm text-gray-600">Low Stock Items</p>
                        </div>
                    </div>
                </Card>

                <Card>
                    <div className="flex items-center">
                        <div className="p-3 rounded-full bg-red-100 mr-4">
                            <svg className="w-6 h-6 text-red-600" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd"/>
                            </svg>
                        </div>
                        <div>
                            <p className="text-2xl font-bold text-gray-900">100</p>
                            <p className="text-sm text-gray-600">Out of Stock</p>
                        </div>
                    </div>
                </Card>
            </div>

            <Card className="mb-8">
                <div className="flex items-center justify-between mb-4">
                    <h2 className="text-xl font-semibold text-gray-900">Recent Products</h2>
                    <Button size="sm">Add New Product</Button>
                </div>

                <Table>
                    <TableHead>
                        <TableHeadCell>Product Name</TableHeadCell>
                        <TableHeadCell>Category</TableHeadCell>
                        <TableHeadCell>Stock</TableHeadCell>
                        <TableHeadCell>Price</TableHeadCell>
                        <TableHeadCell>Status</TableHeadCell>
                        <TableHeadCell>Action</TableHeadCell>
                    </TableHead>
                    <TableBody className="divide-y">
                        <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                            <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                                Apple MacBook Pro 17"
                            </TableCell>
                            <TableCell>Electronics</TableCell>
                            <TableCell>25</TableCell>
                            <TableCell>$2999</TableCell>
                            <TableCell>
                                <Badge color="success">In Stock</Badge>
                            </TableCell>
                            <TableCell>
                                <Button size="xs" color="primary">Edit</Button>
                            </TableCell>
                        </TableRow>
                        <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                            <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                                Microsoft Surface Pro
                            </TableCell>
                            <TableCell>Electronics</TableCell>
                            <TableCell>5</TableCell>
                            <TableCell>$1999</TableCell>
                            <TableCell>
                                <Badge color="warning">Low Stock</Badge>
                            </TableCell>
                            <TableCell>
                                <Button size="xs" color="primary">Edit</Button>
                            </TableCell>
                        </TableRow>
                        <TableRow className="bg-white dark:border-gray-700 dark:bg-gray-800">
                            <TableCell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                                Magic Mouse 2
                            </TableCell>
                            <TableCell>Accessories</TableCell>
                            <TableCell>0</TableCell>
                            <TableCell>$99</TableCell>
                            <TableCell>
                                <Badge color="danger">Out of Stock</Badge>
                            </TableCell>
                            <TableCell>
                                <Button size="xs" color="primary">Edit</Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </Card>
        </div>
    );
}
