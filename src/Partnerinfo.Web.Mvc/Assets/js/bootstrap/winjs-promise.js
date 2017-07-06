// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _WinJS, $, ko) {
    "use strict";

    function Promise(init, thisArg) {
        /// <signature>
        /// <summary>
        /// A promise provides a mechanism to schedule work to be done on a value that
        /// has not yet been computed. It is a convenient abstraction for managing
        /// interactions with asynchronous APIs.
        /// </summary>
        /// <param name="init" type="Function">
        /// The function that is called during construction of the  promise. The function
        /// is given three arguments (complete, error, progress). Inside this function
        /// you should add event listeners for the notifications supported by this value.
        /// </param>
        /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
        /// <returns type="$.Deferred().promise()" />
        /// </signature>
        return $.Deferred(function (deferred) {
            try {
                init.apply(thisArg, [deferred.resolve, deferred.reject, deferred.notify]);
            } catch (ex) {
                deferred.rejectWith(thisArg, [ex]);
                _WinJS.DEBUG && _WinJS.log({ type: "error", message: ex.message });
            }
        }).promise();
    }

    Promise.timeout = function Promise_timeout(timeout, thisArg) {
        /// <signature>
        /// <summary>Creates a promise that is fulfilled after a timeout.</summary>
        /// <param name="timeout" type="Number" optional="true">
        /// The timeout period in milliseconds. If this value is zero or not specified
        /// setImmediate is called, otherwise setTimeout is called.
        /// </param>
        /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
        /// <returns type="WinJS.Promise">
        /// A promise that is completed asynchronously after the specified timeout.
        /// </returns>
        /// </signature>
        return Promise(function (completeDispatch) {
            /* jslint bitwise: true */
            var handle = _Global.setTimeout(function () {
                _Global.clearTimeout(handle);
                completeDispatch.apply(thisArg, []);
            }, timeout | 0);
        }, thisArg);
    };

    Promise.debounce = function Promise_debounce(callback, timeout) {
        /// <signature>
        /// <summary>Returns a new function that will execute only once, coalescing multiple sequential calls into a single execution.</summary>
        /// <param name="callback" type="Function">A callback you want to execute after delay milliseconds.</param>
        /// <param name="timeout" type="Number" optional="true">
        /// The timeout period in milliseconds. If this value is zero or not specified
        /// setImmediate is called, otherwise setTimeout is called.
        /// </param>
        /// <returns type="Function" />
        /// </signature>
        var handle = null;
        var reject = null;
        return function () {
            var thisArg = this;
            var args = Array.prototype.slice.apply(arguments, [0]);
            handle && _Global.clearTimeout(handle);
            reject && reject();
            return Promise(function (completeDispatch, errorDispatch) {
                reject = errorDispatch;
                handle = _Global.setTimeout(function () {
                    var result;
                    try {
                        result = callback.apply(thisArg, args);
                    }
                    catch (ex) {
                        errorDispatch();
                        return;
                    }
                    if (result && result.then) {
                        result.then(completeDispatch, errorDispatch);
                    } else {
                        completeDispatch();
                    }
                }, timeout | 0);
            });
        };
    };

    Promise.join = function Promise_join(values) {
        /// <signature>
        /// <summary>Creates a promise that is fulfilled when all the values are fulfilled.</summary>
        /// <param name="values" type="Array">An object whose members contain values, some of which may be promises.</param>
        /// <returns type="WinJS.Promise" />
        /// </signature>
        return $.when.apply(this, values || []);
    };

    Promise.complete = function Promise_complete(value) {
        /// <signature>
        /// <summary>
        /// Wraps a non-promise value in a promise. You can use this function if you need
        /// to pass a value to a function that requires a promise.
        /// </summary>
        /// <param name="value" optional="true">Some non-promise value to be wrapped in a promise.</param>
        /// <returns type="WinJS.Promise">
        /// A promise that is successfully fulfilled with the specified value
        /// </returns>
        /// </signature>
        return Promise(function (completeDispatch) {
            completeDispatch(value);
        });
    };

    Promise.error = function Promise_error(error) {
        /// <signature>
        /// <summary>
        /// Wraps a non-promise error value in a promise. You can use this function if you need
        /// to pass an error to a function that requires a promise.
        /// </summary>
        /// <param name="error" optional="true">A non-promise error value to be wrapped in a promise.</param>
        /// <returns type="WinJS.Promise">
        /// A promise that is in an error state with the specified value.
        /// </returns>
        /// </signature> 
        return Promise(function (completeDispatch, errorDispatch) {
            errorDispatch(error);
        });
    };

    Promise.tasks = function (target) {
        /// <signature>
        /// <summary>Defines a task array on the specified target object.</summary>
        /// <param name="target" type="Object" />        
        /// </signature>
        var _disposed = false;

        target.tasks = ko.observableArray();
        target.busy = ko.pureComputed(function () { return this.tasks().length > 0; }, target);
        target.then = function (doneCallbacks, failCallbacks) { return Promise.join(target.tasks()).then(doneCallbacks, failCallbacks); };
        target.done = function (doneCallbacks) { return Promise.join(target.tasks()).done(doneCallbacks); };
        target.fail = function (failCallbacks) { return Promise.join(target.tasks()).fail(failCallbacks); };

        return {
            dispose: function () {
                if (_disposed) {
                    return;
                }
                this.busy && this.busy.dispose();
                this.busy = null;
                _disposed = true;
            }
        };
    };

    Promise.tasks.watch = function (asyncCallback) {
        /// <signature>
        /// <summary>Creates a function that tracks the lifetime of the registered tasks.</summary>
        /// <param name="asyncCallback" type="Function" />
        /// <returns type="Function" />
        /// </signature>
        return function () {
            var that = this;
            var promise = asyncCallback.apply(this, arguments);
            if (promise.state() === "pending") {
                this.tasks.push(promise);
                promise = promise.always(function () { that.tasks.remove(promise); });
            }
            return promise;
        };
    };

    _WinJS.Promise = Promise;

})(window, WinJS, jQuery, ko);
