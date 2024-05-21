<script setup>
import "@/common/assets/layout/forms.scss";
import { ref, reactive } from 'vue';

const selectedDetails = reactive({
  clientId: 0, roomId: -1, timeslotId: 0, weekNum: 0, treatmentId: 0
});

const clientData = ref([{name: 'mr. name', id: 2}, {name: 'other.name', id: 5}]);
const treatmentData = ref([{name: 'treatment1', id: 2}, {name: 'treatment2', id: 5}]);
const roomData = ref([{name: 'None', id: -1},{name: 'roomName', building: 'roomBldg', id: 9}]);

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
                <Dropdown id="clientSelect" v-model="selectedDetails.clientId" :options="clientData" filter showClear optionLabel="name" optionValue="id" placeholder="Select a Client" class="w-full">
                </Dropdown>
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="treatmentSelect">Treatment</label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-receipt"></i>
              </InputGroupAddon>
                <Dropdown id="treatmentSelect" v-model="selectedDetails.treatmentId" :options="treatmentData" filter showClear optionLabel="name" optionValue="id" placeholder="Select a Treatment" class="w-full">
                </Dropdown>
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="roomSelect">Room</label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-warehouse"></i>
              </InputGroupAddon>
                <Dropdown id="roomSelect" v-model="selectedDetails.roomId" :options="roomData" filter optionLabel="name" optionValue="id" placeholder="Select a Room" class="w-full">
                </Dropdown>
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking">
            <label for="dtSelect">When</label>
            <InputGroup style="max-width:20rem">
              <InputGroupAddon>
                <i class="pi pi-clock"></i>
              </InputGroupAddon>
                <Calendar id="dtSelect" v-model="selectedDetails.date" :manualInput="false" />
            </InputGroup>
          </fieldset>
          
          <fieldset class="quick-booking quick-booking-end">
            <Button title="Click to review before confirming your booking on the next screen.">Book Client...</Button>
          </fieldset>
        </div>
    </Panel>
  </main>
</template>

<style>
</style>