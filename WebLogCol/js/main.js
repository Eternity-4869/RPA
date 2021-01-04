let infoArr = [];
let browserInfo = [];
let userActionInfo = [];

window.onload = function() {
    initial();
}

function initial() {
    // let startBtn = document.getElementById("start");
    let allBtn = document.getElementById("all");
    let latestBtn = document.getElementById("latest");
    // startBtn.addEventListener("click", () => {
    //     collectInfo();
    // });
    collectInfo();
    allBtn.addEventListener("click", () => {
        chrome.runtime.sendMessage({
            sender: 'exportall',
            data: infoArr,
        }, (res) => {
            console.log("main 收到")
            console.log(res);
            const { data } = res;
            browserInfo = data.browserInfo;
            userActionInfo = data.userActionInfo;
            exportAll();
        })
    })
    latestBtn.addEventListener("click", () => {
        chrome.runtime.sendMessage({
            sender: 'exportlatest',
            data: infoArr,
        }, (res) => {
            //console.log("main shoudao ")
            console.log(res);
            const { data } = res;
            browserInfo = data.browserInfo;
            userActionInfo = data.userActionInfo;

            //userActionInfo存储了所有的用户操作
            console.log(userActionInfo);


            exportLatest();
        })
    })
}

function getCurrentTabId(callback)
{//获取当前标签的信息
	chrome.tabs.query({active: true, currentWindow: true}, function(tabs)
	{
		//console.log(tabs);
		if(callback) callback(tabs.length ? tabs[0]: null);
	});
}

function getWebInfo(callback) {
    chrome.windows.getCurrent({
        populate: true,
    }, callback)
}

function collectInfo() {
    let info = {};
    getWebInfo(function(window) {
        //alert("收集成功");
        info.webinfo = window;
        //console.log(info);
        //exportCsv(info, 'webinfo.csv');
        infoArr.push(info);

        chrome.runtime.sendMessage({
            sender: 'browser',
            data: infoArr,
        }, (res) => {
            console.log("收到返回结果");
            console.log(res);
        })
    });
}


//输出数据文件
// function arrayToCsv(data, args = {}) {
//     let columnDelimiter = args.columnDelimiter || ',';
//     let lineDelimiter = args.lineDelimiter || '\n';

//     return data.reduce((csv, row) => {
//         const rowContent = Array.isArray(row)
//             ? row.reduce((rowTemp, col) => {
//                 let ret = rowTemp ? rowTemp + columnDelimiter : rowTemp;
                
//                 let formatedCol = col.toString().replace(new RegExp(lineDelimiter, 'g'), ' ');
//                 ret += /,/.test(formatedCol) ? `"${formatedCol}"` : formatedCol;
            
//                 return ret;
//             }, '')
//             : row;
//         return (csv ? csv + lineDelimiter : '') + rowContent;
//     }, '');
// }

// const BOM = '\uFEFF';
// function objToArray(obj) {
//     let arr = [];
//     for (const [key, value] of Object.entries(obj)) {
//         if (typeof value === 'object') {
//             arr.push(objToArray(value));
//         }
//         else {
//             arr.push([key, value]);
//         }
//     }
//     return arr;
// }

// function objToArray(obj) {
//     let arr = [];
//     for (const [key, value] of Object.entries(obj)) {
//         if (typeof value === 'undefined') {
//             arr.push([key, 'undefined']);
//         }
//         else if (!value && typeof value != "undefined" && value != 0) {
//             arr.push([key, 'null']);
//         }
//         else if (Object.prototype.toString.call(value) === '[object Object]') {
//             arr.push(objToArray(value));
//         }
//         else if (Object.prototype.toString.call(value) === '[object Array]') {
//             console.log('arr');

//             console.log(JSON.stringify(value).replace(/,/g, ' '));
//             arr.push([key, JSON.stringify(value).replace(/,/g, ' ')]);
//         }
//         else {
//             arr.push([key, value]);
//         }
//     }
//     return arr;
// }

// function objToFormattedArray(obj) {//转换数据格式
//     let arr = objToArray(obj);

//     arr = arr.flat(Infinity);
//     let keys = [];
//     let values = [];
//     for (let i = 0; i < arr.length; i ++) {
//         if (i % 2 === 0) {
//             keys.push(arr[i]);
//         }
//         else {
//             values.push(arr[i]);
//         }
//     }
//     console.log([keys, values]);
//     return [keys, values];
// }

// function exportCsv(inputObj, filename = 'export.csv') {//导出csv文件并储存
//     console.log(inputObj);
//     const arr = objToFormattedArray(inputObj)
//     const csv = arrayToCsv(arr);

//     if (navigator.msSaveOrOpenBlob) {
//         let blob = new Blob([BOM + csv], { type: 'text/csv;charset=utf-8;' });
//         navigator.msSaveOrOpenBlob(blob, filename);
//     } else {
//         let uri = encodeURI(`data:text/csv;charset=utf-8,${BOM}${csv}`);
//         let downloadLink = document.createElement('a');
//         downloadLink.href = uri;
//         downloadLink.download = filename;
//         document.body.appendChild(downloadLink);
//         downloadLink.click();
//         document.body.removeChild(downloadLink);
//     }
// }

// function getTime() {
//     const date = new Date();
//     return '_' + date.getFullYear() + '_' + (date.getMonth()+1) + '_' + 
//         date.getDate() + '_' + date.getHours() + '_' + date.getMinutes() +
//         '_' + date.getSeconds();
// }

// function exportLatest() {
//     if (browserInfo.length > 0) {
//         const time = getTime();
//         exportCsv(browserInfo[browserInfo.length - 1], `webinfo${time}_latest.csv`)
//     }
//     if (userActionInfo.length > 0) {
//         const time = getTime();
//         exportCsv(userActionInfo[userActionInfo.length - 1], `userinfo${time}_latest.csv`)
//     }
// }

// function exportAll() {
//     if (browserInfo.length > 0) {
//         let time;
//         for (let i = 0; i < browserInfo.length; i ++) {
//             time = getTime();
//             exportCsv(browserInfo[i], `webinfo${time}_${i+1}.csv`);
//         }
//     }

//     if (userActionInfo.length > 0) {
//         let time;
//         for (let i = 0; i < userActionInfo.length; i ++) {
//             time = getTime();
//             exportCsv(userActionInfo[i], `userinfo${time}-${i+1}.csv`);
//         }
//     }
// }

// function formatUserInfo() {
//     let headers = [];
//     let body = [];
//     console.log(userActionInfo);
// }