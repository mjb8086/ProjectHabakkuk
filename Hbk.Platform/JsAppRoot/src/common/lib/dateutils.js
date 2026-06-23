import {DateTime} from 'luxon';

const DateUtils = {
    getFormattedDateTime: function(isoDateString)
    {
        return DateTime.fromISO(isoDateString).toLocaleString({...DateTime.DATETIME_SHORT, hourCycle: "h11"});
    },
    getFriendlyDateTime: function(isoDateString)
    {
        return DateTime.fromISO(isoDateString).toLocaleString({...DateTime.DATETIME_MED_WITH_WEEKDAY, hourCycle : "h11"});
    }
};

export default DateUtils;