//存储数据
let infoArr = [];

window.onload = function () {
    main();
}

function main() {

    document.addEventListener("click", (e) => {
        //console.log(e);
        let target = e.target;
        let filter = target.getAttribute("filter");
        if (filter === "noaction") {
            return;
        }
        else {
            let elInfo = getElementInfo(target);
            infoArr.push(elInfo);
            //console.log(infoArr);
            chrome.runtime.sendMessage({
                sender: 'content',
                data: infoArr,
            }, (res) => {
                console.log("contentjs 收到返回结果");
                console.log(res);
            })
        }

    });
}

function getPageInfo(element) {
    let info = {};
    info.type = element.nodeType;
    //elData
    let elData = {};
    elData.xpath = readXPath(element);
    let elementInfo = {};
    elementInfo.className = element.className;
    elementInfo.elementId = element.getAttribute("id");
    elementInfo.tag = element.tagName;
    elementInfo.role = element.nodeType;
    elementInfo.text = element.value;
    elementInfo.value = element.nodeValue;
    elementInfo.optionSb = "";
    elData.elementInfo = elementInfo;
    info.elData = elData;

    //webinfo
    let webInfo = {};


    info.webInfo = webInfo;

    let children = element.children;
    let childrenInfo = [];
    if (children.length > 0) {
        for (let i = 0; i < children.length; i++) {
            let temp = getPageInfo(children[i]);
            childrenInfo.push(temp);
        }
    }

    info.childrenInfo = childrenInfo;

    return info;
}

function getElementInfo(element) {
    let info = {};
    info.type = element.nodeType;

    //elData
    let elData = {};
    elData.xpath = readXPath(element);
    let elementInfo = {};
    elementInfo.className = element.className;
    elementInfo.elementId = element.getAttribute("id");
    elementInfo.tag = element.tagName;
    elementInfo.role = element.nodeType;
    elementInfo.text = element.value;
    elementInfo.value = element.nodeValue;
    elementInfo.optionSb = "";
    elData.elementInfo = elementInfo;
    info.elData = elData;

    return info;
}

function readXPath(element) {
    if (element.id !== "") {
        return '//*[@id=\"' + element.id + '\"]';
    }
    if (element == document.body) {
        return '/html/' + element.tagName.toLowerCase();
    }
    var ix = 1,
        siblings = element.parentNode.childNodes;

    for (var i = 0, l = siblings.length; i < l; i++) {
        var sibling = siblings[i];
        if (sibling == element) {
            return arguments.callee(element.parentNode) + '/' + element.tagName.toLowerCase() + '[' + (ix) + ']';
        } else if (sibling.nodeType == 1 && sibling.tagName == element.tagName) {
            ix++;
        }
    }
};
