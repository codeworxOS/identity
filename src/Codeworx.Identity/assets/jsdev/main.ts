const inputs = document.querySelectorAll<HTMLInputElement>(".validation-digit");
const toggles = document.querySelectorAll<HTMLInputElement>(".toggle-pw");

function onToggle(sender: HTMLElement, event: MouseEvent) {
    let children = sender.parentElement.getElementsByTagName("input");
    let toggleOn = false;
    let icon = sender.getElementsByTagName('i').item(0);
    if (icon instanceof HTMLElement) {
        if (icon.classList.contains('fa-eye-slash')) {
            toggleOn = true;
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        } else {
            icon.classList.remove('fa-eye');
            icon.classList.add('fa-eye-slash');
        }
    }

    if (children.length > 0) {
        var input = children.item(0);
        if (toggleOn) {
            if (input.type == 'password') {
                input.type = 'text';
            }
        } else {
            if (input.type == 'text') {
                input.type = 'password';
            }
        }
    }
}

function onPaste(event: ClipboardEvent): void {
    event.stopPropagation();
    event.preventDefault();

    let clipboardData = event.clipboardData;
    let pastedData = clipboardData?.getData('Text');

    if (pastedData) {
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

function showBusyIndicator(): void {
    const indicators = document.querySelectorAll<HTMLInputElement>(".busy-indicator");
    indicators.forEach(p => p.classList.add('active'));
}

function changeButtonsState(disable: boolean): void {
    let items = document.getElementsByTagName('button');
    for (let i = 0; i < items.length; i++) {
        items.item(i).disabled = disable;
    }
}

function setButtonToggleBox(id: string): void {
    changeButtonsState(true);
    let checkBox = document.getElementById(id);
    checkBox.addEventListener('change', p => {
        let box = document.getElementById(id);
        if (box instanceof HTMLInputElement) {
            if (box.checked) {
                changeButtonsState(false);
            } else {
                changeButtonsState(true);
            }
        }
    });

    let buttons = document.getElementsByTagName('button');
    for (let i = 0; i < buttons.length; i++) {
        let child = buttons.item(i).getElementsByTagName("span").item(0);
        if (child) {
            child.addEventListener("click",
                (ev) => {
                    if (ev.target instanceof HTMLSpanElement && ev.target.parentElement instanceof HTMLButtonElement) {
                        if (ev.target.parentElement.disabled === true) {
                            let box = document.getElementById(id);
                            let errors = box.parentElement.getElementsByClassName('error');
                            for (let i = 0; i < errors.length; i++) {
                                let error = errors.item(i);
                                if (error.classList.contains('shakeable')) {
                                    if (error instanceof HTMLDivElement) {
                                        error.classList.remove('hide');
                                    }
                                    error.classList.add('horizontal-shake');
                                    window.setTimeout(function () { error.classList.remove('horizontal-shake'); }, 500);
                                }
                            }
                        }
                    }
                });
        }
    }
}

window.addEventListener('beforeunload', p => showBusyIndicator());

inputs.forEach((input) => {
    input.addEventListener('keyup', onKeyUp);
    input.addEventListener('paste', onPaste);
});

toggles.forEach((input) => {
    input.addEventListener('click', p => onToggle(input, p));
});
