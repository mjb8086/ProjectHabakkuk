import {CurrentUserSettings} from "@/common/lib/globals.js";
import {DateTime} from 'luxon';

const DateUtils = {
    getFormattedDateTime: function(isoDateString)
    {
        const dt = DateTime.fromISO(isoDateString);
        return dt.toFormat("DD t");
    },
    getFriendlyDateTime: function(isoDateString)
    {
        const dt = DateTime.fromISO(isoDateString);
        return dt.toFormat("DD t");
    }
};

export default DateUtils;