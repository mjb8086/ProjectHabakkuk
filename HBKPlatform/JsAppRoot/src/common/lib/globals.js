import {ref} from "vue";
/* 
 *  Common Globals
 *  ---------------
 *  Author: Mark Brown
 *  Authored: 03/05/24
 * 
 *  Reactive global values. Defaults are set for all values here.
 * 
 *  Notes:
 *  ------
 *  If need be, underscore's default filter can fill in missing object properties when fetched from the API.
 *  Typically the values will be replaced with a fetch from the API.
 *  Steady values belong in consts.js
 * 
 *  © 2024 NowDoctor Ltd
 */

export const CurrentUserData = ref({fullUserName: '', roleName: '', profilePicUrl: ''});