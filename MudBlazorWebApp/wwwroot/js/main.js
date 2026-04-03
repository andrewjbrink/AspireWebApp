import { initializeMap } from './mapLoader.js';

export async function initializeMapWrapper() {
    console.log('Website is ready!');

    try {
        const { map, view } = await initializeMap();

        window.map = map;
        window.view = view;

        console.log('Map and view available as window.map and window.view');
    } catch (error) {
        console.error('Failed to initialize map:', error);
    }
}