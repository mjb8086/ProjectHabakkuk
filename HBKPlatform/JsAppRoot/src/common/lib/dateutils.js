import {CurrentUserSettings} from "@/common/lib/globals.js";
import {DateTime} from 'luxon';

const DateUtils = {
    getFormattedDateTime: function(dateTimeStr)
    {
        const dt = DateTime.fromISO(dateTimeStr);
        return dt.toFormat("DD t");
    }
};

export default DateUtils;