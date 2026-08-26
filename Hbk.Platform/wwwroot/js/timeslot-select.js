document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("timeslotPickerForm");
    if (!form) return;

    const dateSelect = document.getElementById("timeslotDate");
    const select = document.getElementById("timeslotSelect");
    const options = document.getElementById("timeslotOptions");
    const timeslotId = document.getElementById("selectedTimeslotId");
    const weekNum = document.getElementById("selectedWeekNum");
    const submit = document.getElementById("reviewTimeslot");

    const resetSelection = () => {
        timeslotId.value = "";
        weekNum.value = "";
        submit.disabled = true;
    };

    const updateTimes = () => {
        select.replaceChildren(new Option(
            dateSelect.value ? "Select a time" : "Select a date first",
            ""
        ));

        if (!dateSelect.value) {
            select.disabled = true;
            resetSelection();
            return;
        }

        options.content.querySelectorAll(`option[data-date="${dateSelect.value}"]`)
            .forEach(option => select.appendChild(option.cloneNode(true)));

        select.disabled = false;
        resetSelection();
    };

    const updateSelection = () => {
        const option = select.options[select.selectedIndex];
        const hasSelection = option.value !== "";

        timeslotId.value = hasSelection ? option.value : "";
        weekNum.value = hasSelection ? option.dataset.weekNum : "";
        submit.disabled = !hasSelection;
    };

    dateSelect.addEventListener("change", updateTimes);
    select.addEventListener("change", updateSelection);
    updateTimes();
});
