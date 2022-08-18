export function Animate(element, path, duration)
{
    let summaryDiff = {
        x: 0,
        y: 0
    };

    for (let i = 0; i < path.length; i++) {
        const diff = path[i];
        setTimeout(function () {
            summaryDiff.x += diff.x;
            summaryDiff.y += diff.y;
            element.style.transform = 'translate(' + (summaryDiff.x * 60) + 'px,' + (summaryDiff.y * 60) + 'px) translateZ(20px)';
        }, (duration / path.length) * i);  
    }
    element.style.transform = "";
}