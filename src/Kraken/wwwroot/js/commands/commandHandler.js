define(['cmdr'], function (cmdr) {

    return new cmdr.CommandHandler({
        contextExtensions: {
            fail: function (xhr, error, message) {
                if (typeof xhr === 'string') {
                    message = xhr;
                }
                if (message) {
                    this.shell.writeLine(message, 'error');
                }
                if (xhr.responseText) {
                    this.shell.writeLine(xhr.responseText, 'error');
                }
                this.shell.writeLine('Operation Failed', 'error');
            }
        }
    });

});