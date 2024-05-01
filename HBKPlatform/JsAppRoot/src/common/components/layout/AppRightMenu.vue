<script setup>
import { ref } from 'vue';
import { useLayout } from '@/common/components/layout/layout.js';

const { layoutState } = useLayout();

const checked1 = ref(true);
const checked2 = ref(true);
const checked3 = ref(false);
const checked4 = ref(false);
const checked5 = ref(false);
const checked6 = ref(false);
const checked7 = ref(false);
const checked8 = ref(false);
</script>

<template>
    <Sidebar v-model:visible="layoutState.rightMenuActive.value" position="right" :baseZIndex="1000" :showCloseIcon="false">
        <div class="h-full py-4 pr-0">
            <div>
                <h5 class="mt-5 mx-3 mb-2 text-color opacity-80">Your Tasks Today</h5>
                <ul class="py-5 m-0 p-0 list-none">
                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked1"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked1 }">Sales reports</h6>
                            <span class="block text-sm text-color opacity-50">Today</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked2"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked2 }">Pay invoices</h6>
                            <span class="block text-sm text-color opacity-50">Today</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked3"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked3 }">Dinner with Tony</h6>
                            <span class="block text-sm text-color opacity-50">Today, 22:36</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked4"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked4 }">Client meeting</h6>
                            <span class="block text-sm text-color opacity-50">Today</span>
                        </div>
                    </li>
                </ul>
            </div>

            <hr />

            <div>
                <h5 class="mt-5 mx-3 mb-2 text-color opacity-80">Upcoming</h5>
                <ul class="py-5 m-0 p-0 list-none">
                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked5"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked5 }">New representative meeting</h6>
                            <span class="block text-sm text-color opacity-50">27 May</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked6"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked6 }">Flight tickets</h6>
                            <span class="block text-sm text-color opacity-50">28 May</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked7"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked7 }">Generate charts</h6>
                            <span class="block text-sm text-color opacity-50">28 May</span>
                        </div>
                    </li>

                    <li class="flex py-2 px-4">
                        <Checkbox :binary="true" v-model="checked8"></Checkbox>
                        <div class="ml-3">
                            <h6 class="m-0 text-color" :class="{ 'line-through': checked8 }">Delete old files</h6>
                            <span class="block text-sm text-color opacity-50">30 May</span>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    </Sidebar>
</template>

<style lang="scss" scoped>
:deep(.p-checkbox) {
    width: 32px;
    height: 32px;

    .p-checkbox-box {
        width: 32px;
        height: 32px;
        border-radius: 50%;
    }

    &.p-highlight {
        .p-checkbox-box {
            color: #515c66;
            background-color: transparent;
            border-color: transparent;
        }

        .p-checkbox-icon {
            color: var(--text-color);
        }

        &:hover {
            border-color: #d9dee6;
        }
    }
}
</style>
