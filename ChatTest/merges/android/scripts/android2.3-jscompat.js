// Polyfill per il supporto di Function.prototype.bind() in Android 2.3
(function () {
    if (!Function.prototype.bind) {
        Function.prototype.bind = function (thisValue) {
            if (typeof this !== "function") {
                throw new TypeError(this + " cannot be bound as it is not a function");
            }

            // bind() consente inoltre di anteporre argomenti alla chiamata
            var preArgs = Array.prototype.slice.call(arguments, 1);

            // Funzione effettiva a cui associare il valore "this" e gli argomenti
            var functionToBind = this;
            var noOpFunction = function () { };

            // Argomento "this" da usare
            var thisArg = this instanceof noOpFunction && thisValue ? this : thisValue;

            // Funzione associata risultante
            var boundFunction = function () {
                return functionToBind.apply(thisArg, preArgs.concat(Array.prototype.slice.call(arguments)));
            };

            noOpFunction.prototype = this.prototype;
            boundFunction.prototype = new noOpFunction();

            return boundFunction;
        };
    }
}());
