//@inject IJSRuntime _jsRuntime
//await _jsRuntime.InvokeVoidAsync("Index.scope.closeModal", id);
//await _jsRuntime.InvokeVoidAsync("Index.log", "OOOOOOOOOOoooooooooo");

declare var bootstrap: any;

module Index {

    export class Modal {
        private modal: any;
       
        openModal(id: string) {
            this.modal = new bootstrap.Modal(document.getElementById(id), {});
            this.modal.show();
        }

        closeModal(id: string) {
            this.modal.hide();
        }
    }

    export let scope = new Modal();

    export function log(s: string) {
        console.log(s);
    }
}