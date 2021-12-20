//@inject IJSRuntime _jsRuntime
//await _jsRuntime.InvokeVoidAsync("Index.scope.closeModal", id);
//await _jsRuntime.InvokeVoidAsync("Index.log", "OOOOOOOOOOoooooooooo");
var Index;
(function (Index) {
    var Modal = /** @class */ (function () {
        function Modal() {
        }
        Modal.prototype.openModal = function (id) {
            this.modal = new bootstrap.Modal(document.getElementById(id), {});
            this.modal.show();
        };
        Modal.prototype.closeModal = function (id) {
            this.modal.hide();
        };
        return Modal;
    }());
    Index.Modal = Modal;
    Index.scope = new Modal();
    function log(s) {
        console.log(s);
    }
    Index.log = log;
})(Index || (Index = {}));
//# sourceMappingURL=app.js.map