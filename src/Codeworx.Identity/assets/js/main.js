var inputs = document.querySelectorAll(".validation-digit");
function onPaste(event) {
    event.stopPropagation();
    event.preventDefault();
    var clipboardData = event.clipboardData;
    var pastedData = clipboardData === null || clipboardData === void 0 ? void 0 : clipboardData.getData('Text');
    if (pastedData) {
        for (var i = 0; i < inputs.length; i++) {
            if (pastedData.length > i) {
                inputs[i].value = pastedData[i];
            }
            else {
                inputs[i].focus();
                break;
            }
        }
    }
}
function onKeyUp(event) {
    var _a;
    if (event.isComposing || event.keyCode === 229) {
        return;
    }
    if (event.key === 'v' && event.ctrlKey) {
        return;
    }
    var target = event.srcElement || event.target;
    if (target instanceof HTMLInputElement) {
        var maxLength = target.maxLength;
        var myLength = target.value.length;
        if (myLength >= maxLength) {
            var next = target;
            while (next = next.nextElementSibling) {
                if (next == null) {
                    break;
                }
                if (next instanceof HTMLInputElement) {
                    next.focus();
                    return;
                }
            }
            if ((_a = target === null || target === void 0 ? void 0 : target.form) === null || _a === void 0 ? void 0 : _a.elements['submit']) {
                var submitButton = target.form.elements['submit'];
                if (submitButton instanceof HTMLInputElement) {
                    submitButton.focus();
                }
            }
        }
        else if (myLength === 0 && event.key === 'Backspace') {
            var previous = target;
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
inputs.forEach(function (input) {
    input.addEventListener('keyup', onKeyUp);
    input.addEventListener('paste', onPaste);
});
