var stockNo;

function GetStockDetails() {
    var name = localStorage.getItem("userName");
    PageMethods.GetStockNo(name, onSuccess, onError);
    function onSuccess(StockNumber) {
        if (StockNumber == "") {
            alert('No Stock Found');
            return;
        }
        document.getElementById("lblstockNo").innerHTML = "Stock Number : " + StockNumber;
        stockNo = StockNumber;
        DisplayImages(stockNo);
    }
    function onError(StockNumber) {
        alert('Something wrong.');
    }
}

function PassFailClick(flag) {
    PageMethods.UpdateStockNo(stockNo, flag, onSuccess, onError);
    function onSuccess(StockNumber) {
    }
    function onError(StockNumber) {
        alert('Something wrong at Button Click.');
    }
}

function DisplayImages(stockNo) {
    PageMethods.GetImages(stockNo, onSuccess, onError);
    function onSuccess(imageUrl) {
        for (i = 0; i < 10; i++) {
            var imgId = 'Image' + (i + 1);
            document.getElementById(imgId).setAttribute('src', imageUrl[i]);
        }
    }
    function onError(imageUrl) {
        alert('Something wrong in displaying Images.');
    }

}