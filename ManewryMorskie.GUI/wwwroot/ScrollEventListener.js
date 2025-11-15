const scrollStep = 10;
let shownModalCount = 0;

async function handleScroll(event, boardTransformService) {

    if (shownModalCount > 0)
        return;

    event.preventDefault();
    event.stopPropagation();

    const stateBeforeChange = await boardTransformService.invokeMethodAsync('GetCurrentState');

    const zoomChange = event.deltaY > 0 ? -scrollStep : scrollStep;
    await boardTransformService.invokeMethodAsync('ChangeZoom', zoomChange);


    const newState = await boardTransformService.invokeMethodAsync('GetCurrentState');
    if (newState.zoom == stateBeforeChange.zoom || zoomChange < 0)
        return;

    const gameBg = document.querySelector('#game-bg');
    gameBg.scrollTo({
        left: event.clientX,
        top: event.clientY,
    });
}

export function registerScrollEventListener(boardTransformService)
{
    document.addEventListener('shown.bs.modal', (e) => {
        shownModalCount++;
    });

    document.addEventListener('hidden.bs.modal', (e) => {
        shownModalCount--;
    });

    document.addEventListener('wheel', async (event) => await handleScroll(event, boardTransformService), { passive: false });
}