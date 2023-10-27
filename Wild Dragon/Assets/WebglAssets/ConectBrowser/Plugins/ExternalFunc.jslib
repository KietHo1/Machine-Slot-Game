mergeInto(LibraryManager.library, {

    GetTerminalID: function () {
        try {
            const params = window.TERMINAL_ID;            
            var bufferSize = lengthBytesUTF8(params) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(params, buffer, bufferSize);
            return buffer;
        } catch (e) {
            console.log(e);
            return '';
        }
    },
    GetShopID: function () {
        try {
            const params = window.SHOP_ID;            
            var bufferSize = lengthBytesUTF8(params) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(params, buffer, bufferSize);
            return buffer;
        } catch (e) {
            console.log(e);
            return '';
        }
    },
    GetGameID: function () {
        try {
            const params = window.GAME_ID;            
            var bufferSize = lengthBytesUTF8(params) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(params, buffer, bufferSize);
            return buffer;
        } catch (e) {
            console.log(e);
            return '';
        }
    },
});