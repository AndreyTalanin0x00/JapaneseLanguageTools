const mode = import.meta.env.MODE;

export const isDevelopmentMode = () => mode.toLowerCase() === "development";
export const isProductionMode = () => mode.toLowerCase() === "production";

export const developmentServerPort = 5238;
