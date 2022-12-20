const inputs = document.querySelectorAll<HTMLInputElement>(".validation-digit");

function onPaste(event: ClipboardEvent): void {
    event.stopPropagation();
    event.preventDefault();

    let clipboardData = event.clipboardData;
    let pastedData = clipboardData?.getData('Text');

    if(pastedData){
        for (let i = 0; i < inputs.length; i++) {
            if (pastedData.length > i) {
                inputs[i].value = pastedData[i];
            } else {
                inputs[i].focus();
                break;
            }
        }
    }
}

function onKeyUp(event: KeyboardEvent): void {
    if (event.isComposing || event.keyCode === 229) {
        return;
    }

    if (event.key === 'v' && event.ctrlKey) {
        return;
    }

    let target = event.srcElement || event.target;

    if (target instanceof HTMLInputElement) {
        let maxLength = target.maxLength;
        let myLength = target.value.length;
        if (myLength >= maxLength) {
            let next: Element | null = target;
            while (next = next.nextElementSibling) {
                if (next == null) {
                    break;
                }
                if (next instanceof HTMLInputElement) {
                    next.focus();
                    return;
                }
            }
            if (target?.form?.elements['submit']) {
                let submitButton = target.form.elements['submit']
                if (submitButton instanceof HTMLInputElement) {
                    submitButton.focus();
                }
            }
        }
        else if (myLength === 0 && event.key === 'Backspace') {
            let previous: Element | null = target;
            while (previous = previous.previousElementSibling) {
                if (previous == null)
                    break;
                if (previous instanceof HTMLInputElement) {
                    previous.focus();
                    previous.select();
                    break;
                }
            }
        }
    }
}

inputs.forEach((input) => {
    input.addEventListener('keyup', onKeyUp);
    input.addEventListener('paste', onPaste);
});
