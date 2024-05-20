const Consts = {
    // The basePaths need to match the routes in Program.cs
    CLIENT_BASE_PATH : '/client/ui',
    MYND_BASE_PATH : '/mynd/ui',
    // App logos
    LOGO_MYND: 'MyNDLogo1.png',
    LOGO_CLINIC: 'NDClinicLogo1.png',
    LOGO_CLIENT: 'NDClientLogo1.png',
    APP_LOGO: '', // Intended to be assigned on startup
    MYND_BASE_TITLE: 'MyND '
};

export default Consts;
export const API_BASE = import.meta.env.VITE_API_BASE;
