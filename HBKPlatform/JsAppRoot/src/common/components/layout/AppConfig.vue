<script setup>
import Sidebar from 'primevue/sidebar';
import { usePrimeVue } from 'primevue/config';
import { ref, watch, computed } from 'vue';
import { useLayout } from '@/common/components/layout/layout.js';

defineProps({
    simple: {
        type: Boolean,
        default: false
    }
});

const $primevue = usePrimeVue();
const rippleActive = computed(() => $primevue.config.ripple);
const inputStyle = computed(() => $primevue.config.inputStyle || 'outlined');
const { setScale, layoutConfig, layoutState, onConfigSidebarToggle } = useLayout();

const componentThemes = ref([
    { name: 'denim', color: 'rgb(47, 142, 229)' },
    { name: 'sea-green', color: 'rgb(48, 160, 89)' },
    { name: 'amethyst', color: 'rgb(131, 76, 168)' },
    { name: 'wedgewood', color: 'rgb(85, 125, 170)' },
    { name: 'tapestry', color: 'rgb(167, 72, 150)' },
    { name: 'cape-palliser', color: 'rgb(164, 107, 62)' },
    { name: 'apple', color: 'rgb(82, 162, 53)' },
    { name: 'gigas', color: 'rgb(87, 81, 169)' },
    { name: 'jungle-green', color: 'rgb(43, 159, 156)' },
    { name: 'camelot', color: 'rgb(165, 67, 87)' },
    { name: 'amber', color: 'rgb(212, 147, 65)' },
    { name: 'cyan', color: 'rgb(57, 157, 178)' }
]);

const scales = ref([12, 13, 14, 15, 16]);

watch(layoutConfig.menuMode, (newVal) => {
    if (newVal === 'static') {
        layoutState.staticMenuDesktopInactive.value = false;
    }
});

const colorScheme = ref(layoutConfig.colorScheme.value);

const changeColorScheme = (colorScheme) => {
    const themeLink = document.getElementById('theme-link');
    const themeLinkHref = themeLink.getAttribute('href');
    const currentColorScheme = 'theme-' + layoutConfig.colorScheme.value.toString();
    const newColorScheme = 'theme-' + colorScheme;
    const newHref = themeLinkHref.replace(currentColorScheme, newColorScheme);

    replaceLink(themeLink, newHref, () => {
        layoutConfig.colorScheme.value = colorScheme;
        layoutConfig.menuTheme.value = colorScheme;
        layoutConfig.topbarTheme.value = colorScheme;
    });
};

const changeTheme = (theme) => {
    const themeLink = document.getElementById('theme-link');
    const themeHref = themeLink.getAttribute('href');
    const newHref = themeHref.replace(layoutConfig.theme.value, theme);

    replaceLink(themeLink, newHref, () => {
        layoutConfig.theme.value = theme;
    });
};

const changeMenuTheme = (theme) => {
    layoutConfig.menuTheme.value = theme;
};

const changeTopbarTheme = (theme) => {
    layoutConfig.topbarTheme.value = theme;
};

const replaceLink = (linkElement, href, onComplete) => {
    if (!linkElement || !href) {
        return;
    }

    const id = linkElement.getAttribute('id');
    const cloneLinkElement = linkElement.cloneNode(true);

    cloneLinkElement.setAttribute('href', href);
    cloneLinkElement.setAttribute('id', id + '-clone');

    linkElement.parentNode.insertBefore(cloneLinkElement, linkElement.nextSibling);

    cloneLinkElement.addEventListener('load', () => {
        linkElement.remove();

        const element = document.getElementById(id);
        element && element.remove();

        cloneLinkElement.setAttribute('id', id);
        onComplete && onComplete();
    });
};
const decrementScale = () => {
    setScale(layoutConfig.scale.value - 1);
    applyScale();
};
const incrementScale = () => {
    setScale(layoutConfig.scale.value + 1);
    applyScale();
};

const applyScale = () => {
    document.documentElement.style.fontSize = layoutConfig.scale.value + 'px';
};

const onInputStyleChange = (value) => {
    $primevue.config.inputStyle = value;
};
const onRippleChange = (value) => {
    $primevue.config.ripple = value;
};
</script>

<template>
    <div class="layout-config">
        <Button class="layout-config-button" icon="pi pi-cog" @click="onConfigSidebarToggle()" />
        <Sidebar v-model:visible="layoutState.configSidebarVisible.value" position="right" pt:closeButton="ml-auto">
            <div class="p-3">
                <h5>Layout/Theme Scale</h5>
                <div class="flex align-items-center">
                    <Button icon="pi pi-minus" type="button" @click="decrementScale()" class="p-button-text p-button-rounded w-2rem h-2rem mr-2" :disabled="layoutConfig.scale.value === scales[0]"></Button>
                    <div class="flex gap-3 align-items-center">
                        <i class="pi pi-circle-fill text-300" v-for="s in scales" :key="s" :class="{ 'text-primary-500': s === layoutConfig.scale.value }"></i>
                    </div>
                    <Button icon="pi pi-plus" type="button" @click="incrementScale()" class="p-button-text p-button-rounded w-2rem h-2rem ml-2" :disabled="layoutConfig.scale.value === scales[scales.length - 1]"></Button>
                </div>

                <h5>Color Scheme</h5>
                <div class="flex gap-4">
                    <div class="field-radiobutton flex-1">
                        <RadioButton name="colorScheme" value="light" v-model="colorScheme" id="theme3" @change="changeColorScheme('light')"></RadioButton>
                        <label for="theme3">Light</label>
                    </div>

                    <div class="field-radiobutton flex-1">
                        <RadioButton name="colorScheme" value="dark" v-model="colorScheme" id="theme1" @change="changeColorScheme('dark')"></RadioButton>
                        <label for="theme1">Dark</label>
                    </div>

                    <div class="field-radiobutton flex-1">
                        <RadioButton name="colorScheme" value="dim" v-model="colorScheme" id="theme2" @change="changeColorScheme('dim')"></RadioButton>
                        <label for="theme2">Dim</label>
                    </div>
                </div>
                <template v-if="!simple">
                    <h5>Menu Themes</h5>
                    <div class="flex justify-content-between">
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="menuTheme" value="light" v-model="layoutConfig.menuTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme1" @change="changeMenuTheme('light')"></RadioButton>
                            <label for="mode1" class="ml-2">Light</label>
                        </div>
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="menuTheme" value="dark" v-model="layoutConfig.menuTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme2" @change="changeMenuTheme('dark')"></RadioButton>
                            <label for="mode2" class="ml-2">Dark</label>
                        </div>
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="menuTheme" value="dim" v-model="layoutConfig.menuTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme3" @change="changeMenuTheme('dim')"></RadioButton>
                            <label for="mode4" class="ml-2">Dim</label>
                        </div>
                    </div>

                    <h5>Topbar Themes</h5>
                    <div class="flex justify-content-between">
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="topbarTheme" value="light" v-model="layoutConfig.topbarTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme1" @change="changeTopbarTheme('light')"></RadioButton>
                            <label for="mode1" class="ml-2">Light</label>
                        </div>
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="topbarTheme" value="dark" v-model="layoutConfig.topbarTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme2" @change="changeTopbarTheme('dark')"></RadioButton>
                            <label for="mode2" class="ml-2">Dark</label>
                        </div>
                        <div class="field-radiobutton flex-1">
                            <RadioButton name="topbarTheme" value="dim" v-model="layoutConfig.topbarTheme.value" :disabled="colorScheme !== 'light'" id="menuTheme3" @change="changeTopbarTheme('dim')"></RadioButton>
                            <label for="mode4" class="ml-2">Dim</label>
                        </div>
                    </div>

                    <h5>Menu Mode</h5>
                    <div class="flex flex-wrap row-gap-3">
                        <div class="flex align-items-center gap-2 w-6">
                            <RadioButton name="menuMode" value="static" v-model="layoutConfig.menuMode.value" inputId="mode1"></RadioButton>
                            <label for="mode1">Static</label>
                        </div>
                        <div class="flex align-items-center gap-2 w-6">
                            <RadioButton name="menuMode" value="overlay" v-model="layoutConfig.menuMode.value" inputId="mode2"></RadioButton>
                            <label for="mode2">Overlay</label>
                        </div>
                        <div class="flex align-items-center gap-2 w-6">
                            <RadioButton name="menuMode" value="horizontal" v-model="layoutConfig.menuMode.value" inputId="mode3"></RadioButton>
                            <label for="mode3">Horizontal</label>
                        </div>
                    </div>

                    <h5>Input Style</h5>
                    <div class="flex">
                        <div class="field-radiobutton flex-1">
                            <RadioButton :modelValue="inputStyle" name="inputStyle" value="outlined" inputId="outlined_input" @update:modelValue="onInputStyleChange"></RadioButton>
                            <label for="outlined_input">Outlined</label>
                        </div>
                        <div class="field-radiobutton flex-1">
                            <RadioButton :modelValue="inputStyle" name="inputStyle" value="filled" inputId="filled_input" @update:modelValue="onInputStyleChange"></RadioButton>
                            <label for="filled_input">Filled</label>
                        </div>
                    </div>
                </template>

                <h5>Ripple Effect</h5>
                <InputSwitch :modelValue="rippleActive" @update:modelValue="onRippleChange"></InputSwitch>

                <h5>Component Themes</h5>
                <div class="flex flex-wrap row-gap-3">
                    <div v-for="(theme, i) in componentThemes" :key="i" class="w-3">
                        <a
                            :autoFocus="layoutConfig.theme === theme.name"
                            @click="() => changeTheme(theme.name)"
                            :style="{ 'background-color': theme.color }"
                            class="cursor-pointer p-link w-2rem h-2rem border-round shadow-2 flex-shrink-0 flex justify-content-center align-items-center border-circle"
                        >
                            <i v-if="theme.name === layoutConfig.theme.value" class="pi pi-check text-white"></i>
                        </a>
                    </div>
                </div>
            </div>
        </Sidebar>
    </div>
</template>
