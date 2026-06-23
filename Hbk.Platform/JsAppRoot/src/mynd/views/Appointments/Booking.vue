<script setup>
import "@/common/assets/layout/forms.scss";
import { ref, reactive } from 'vue';
import { API_BASE, ENUM_ROOM_SELECTION, MAX_APPOINTMENT_DURATION, MIN_APPOINTMENT_DURATION } from "@/common/lib/consts.js";
import { FormatCurrency } from "@/common/lib/format-helpers.js";
import Notification from '@/common/lib/toast-wrapper.js';

// Model and Data
const bookingDetails = reactive({
  clientId: 0, locationOption: ENUM_ROOM_SELECTION.Home, roomId: -1, timeslotId: 0, weekNum: 0, treatmentId: 0, note: "",
  meetingUrl: "", date: new Date('2024-05-22'), time: new Date('1970-01-01 15:00'), duration: 5
});

const clientData = ref([]);
const treatmentData = ref([]);
const roomData = ref([]);
let roomsFetched = false;

const locationOptionOptions = ref([
    { name: 'Home', value: ENUM_ROOM_SELECTION.Home },
    { name: 'Clinic', value: ENUM_ROOM_SELECTION.Clinic },
    { name: 'Online', value: ENUM_ROOM_SELECTION.Online },
    { name: 'N/A', value: ENUM_ROOM_SELECTION.NotApplicable }
]);

// API Fetches
fetch(`${API_BASE}/api/mynd/client/getlite`).then((res) => res.json())
    .then((json) => {
      clientData.value = json;
    }).catch((err) => (Notification.error('', err.message)));
fetch(`${API_BASE}/api/mynd/treatment/getlite`).then((res) => res.json())
    .then((json) => {
      treatmentData.value = json;
    }).catch((err) => (Notification.error('', err.message)));

// Event Handlers
// Only fetch room data if room selected
function changeLocationOption(e) {
   if (e.value === ENUM_ROOM_SELECTION.Clinic && !roomsFetched) {
     fetch(`${API_BASE}/api/mynd/roomreservation/getreservationslite`).then((res) => res.json())
         .then((json) => {
           roomData.value = json;
           roomsFetched = true;
         }).catch((err) => (Notification.error('', err.message)));
   }
};

</script>

<template>
  <main>
    <Panel header="Appointment Booking">
    <p>Book an appointment for a client using the quick booking form.</p>
        <div class="card quick-booking-container">
          <fieldset class="quick-booking">
            <label for="clientSelect">Client Name</label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-user"></i>
              </InputGroupAddon>
                <Dropdown id="clientSelect" v-model="bookingDetails.clientId" :options="clientData" filter showClear optionLabel="name" optionValue="id" placeholder="Select a Client" class="w-full">
                </Dropdown>
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="treatmentSelect">Treatment</label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-receipt"></i>
              </InputGroupAddon>
                <Dropdown id="treatmentSelect" v-model="bookingDetails.treatmentId" :options="treatmentData" filter showClear optionLabel="title" optionValue="id" placeholder="Select a Treatment" class="w-full">
                  <template #option="slotProps">
                    <div class="treatment-select">
                      <div>{{ slotProps.option.title }}</div> <div>{{ FormatCurrency(slotProps.option.cost) }}</div>
                    </div>
                  </template>
                </Dropdown>
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="roomSelect">Location</label>
            <SelectButton @change="changeLocationOption" id="locationOption" v-model="bookingDetails.locationOption" :options="locationOptionOptions" optionLabel="name" optionValue="value" :allowEmpty="false"/>
            
            <InputGroup v-if="bookingDetails.locationOption === ENUM_ROOM_SELECTION.Clinic">
              <InputGroupAddon>
                <i class="pi pi-warehouse"></i>
              </InputGroupAddon>
                <Dropdown id="roomSelect" v-model="bookingDetails.roomId" :options="roomData" filter optionLabel="roomTitle" optionValue="id" placeholder="Select a Room" class="w-full">
                </Dropdown>
            </InputGroup>

            <InputGroup v-else-if="bookingDetails.locationOption === ENUM_ROOM_SELECTION.Online">
              <InputGroupAddon>
                <i class="pi pi-link"></i>
              </InputGroupAddon>
              <InputText id="urlField" type="text" v-model="bookingDetails.meetingUrl" placeholder="https://www.example.com" />
            </InputGroup>

          </fieldset>
          
          <fieldset class="quick-booking">
            <label>When</label>

            <div class="formgrid grid">
              <div class="field col">
                <label for="daySelect">Day</label>
                <InputGroup class="half-width-field">
                  <InputGroupAddon>
                    <i class="pi pi-calendar"></i>
                  </InputGroupAddon>
                  <Calendar id="daySelect" v-model="bookingDetails.date" :manualInput="false" />
                </InputGroup>
              </div>
              <div class="field col">
                <label for="timeSelect">Time</label>
                <InputGroup class="half-width-field">
                  <InputGroupAddon>
                    <i class="pi pi-clock"></i>
                  </InputGroupAddon>
                  <Calendar id="timeSelect" v-model="bookingDetails.time" timeOnly />
                </InputGroup>
              </div>
            </div>

            <div class="formgrid grid">
              <div class="field col">
              <label for="duration">Duration (minutes)</label>
              <InputGroup>
                <InputGroupAddon>
                  <i class="pi pi-stopwatch"></i>
                </InputGroupAddon>
                <InputNumber id="duration" v-model="bookingDetails.duration" showButtons :step="5" inputId="integeronly" :max="MAX_APPOINTMENT_DURATION" :min="MIN_APPOINTMENT_DURATION" />
              </InputGroup>
              </div>
            </div>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="clientNote">Note</label>
            <Textarea id="clientNote" v-model="bookingDetails.note" placeholder="Enter a quick note or reminder to be shared with the client." :autoResize="true" rows="5" cols="30" />
          </fieldset>
          
          <fieldset class="quick-booking quick-booking-end">
            <Button @click="Notification.info('', 'nothing')" title="Click to review before confirming your booking on the next screen.">Preview Booking...</Button>
          </fieldset>
        </div>
    </Panel>
  </main>
</template>

<style scoped>
textarea#clientNote {
  width: 100%;
}
div#locationOption {
  margin-bottom: 8px;
}
div.treatment-select {
  display: flex;
  justify-content: space-between;
  width: 100%;
}
</style>