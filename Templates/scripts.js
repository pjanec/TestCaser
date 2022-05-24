"use strict";
// pretty-print-json ~ MIT License
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};

var prettyPrintJson = {
    toHtml: function (thing, options) {
        var defaults = { indent: 3, lineNumbers: false, linkUrls: true, quoteKeys: false };
        var settings = __assign(__assign({}, defaults), options);
        var htmlEntities = function (text) { return text
            // Makes text displayable in browsers.
            .replace(/&/g, '&amp;')
            .replace(/\\"/g, '&bsol;&quot;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;'); };
        var spanTag = function (type, display) {
            // Creates HTML to display a value like: like "<span class=json-boolean>true</span>"
            return display ? '<span class=json-' + type + '>' + display + '</span>' : '';
        };
        var buildValueHtml = function (value) {
            // Analyzes a value and returns HTML like: "<span class=json-number>3.1415</span>"
            var strType = /^"/.test(value) && 'string';
            var boolType = ['true', 'false'].includes(value) && 'boolean';
            var nullType = value === 'null' && 'null';
            var type = boolType || nullType || strType || 'number';
            var urlRegex = /https?:\/\/[^\s"]+/g;
            var makeLink = function (link) { return '<a class=json-link href="' + link + '">' + link + '</a>'; };
            var display = strType && settings.linkUrls ? value.replace(urlRegex, makeLink) : value;
            return spanTag(type, display);
        };
        var replacer = function (match, p1, p2, p3, p4) {
            // Converts the four parenthesized capture groups (indent, key, value, end) into HTML.
            var part = { indent: p1, key: p2, value: p3, end: p4 };
            var findName = settings.quoteKeys ? /(.*)(): / : /"([\w$]+)": |(.*): /;
            var indentHtml = part.indent || '';
            var keyName = part.key && part.key.replace(findName, '$1$2');
            var keyHtml = part.key ? spanTag('key', keyName) + spanTag('mark', ': ') : '';
            var valueHtml = part.value ? buildValueHtml(part.value) : '';
            var endHtml = spanTag('mark', part.end);
            return indentHtml + keyHtml + valueHtml + endHtml;
        };
        var jsonLine = /^( *)("[^"]+": )?("[^"]*"|[\w.+-]*)?([{}[\],]*)?$/mg;
        // Regex parses each line of the JSON string into four parts:
        //    Capture group       Part        Description                  Example
        //    ------------------  ----------  ---------------------------  --------------------
        //    ( *)                p1: indent  Spaces for indentation       '   '
        //    ("[^"]+": )         p2: key     Key name                     '"active": '
        //    ("[^"]*"|[\w.+-]*)  p3: value   Key value                    'true'
        //    ([{}[\],]*)         p4: end     Line termination characters  ','
        // For example, '   "active": true,' is parsed into: ['   ', '"active": ', 'true', ',']
        var json = JSON.stringify(thing, null, settings.indent) || 'undefined';
        var html = htmlEntities(json).replace(jsonLine, replacer);
        var makeLine = function (line) { return "   <li>".concat(line, "</li>"); };
        var addLineNumbers = function (html) {
            return __spreadArray(__spreadArray(['<ol class=json-lines>'], html.split('\n').map(makeLine), true), ['</ol>'], false).join('\n');
        };
        return settings.lineNumbers ? addLineNumbers(html) : html;
    },
};
