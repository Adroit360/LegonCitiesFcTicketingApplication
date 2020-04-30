document.addEventListener("DOMContentLoaded", function (event) {
    var voucherbox = document.getElementById("voucher");
    var phonebox = document.getElementById("phone");
    var matchbox = document.getElementById("match");
    var categorybox = document.getElementById("category");
    var btnVerify = document.getElementById("btnverify");
    var messageBox = document.getElementById("message-box");


    btnVerify.addEventListener("click", () => {
        let voucher = voucherbox.value;
        if (!voucher)
            showErrorInMessageBox("Please Enter the ticket number");
        else
        getrecordswithvoucher(voucher);
    });

    function getrecordswithvoucher(voucher) {
        var xhr = new XMLHttpRequest();
        xhr.responseType = "json";
        xhr.open("GET", `${window.location.origin}/api/transaction/getrecordswithvoucher/${voucher}`);
        xhr.onload = function () {
            digestResponse(xhr.response, xhr.status);
            btnVerify.innerText = "Verify";
            btnVerify.disabled = false;
        };
        btnVerify.disabled = true;
        btnVerify.innerText = "Verifying...";
        xhr.send();

        
    }

    function digestResponse(response, status) {
        console.log(status);
        if (status === 200) {
            showSuccessOutputInDOM(response);
        } else {
            showErrorOutupInDOM(response);
        }
    }

    function showSuccessOutputInDOM(voucherDetails) {
        matchbox.innerText = voucherDetails.match;
        categorybox.innerText = voucherDetails.category;
        phonebox.innerText = voucherDetails.phone;

        if (voucherDetails.isVerified) {
            showErrorInMessageBox("Ticket Number Already Verified");
        } else {
            showSuccessInMessageBox("Verification Successful");
        }

    }

    function showErrorOutupInDOM(voucherDetails) {
        showErrorInMessageBox(voucherDetails.error);
    }

    function showSuccessInMessageBox(message) {
        messageBox.style.backgroundColor = "#089208";
        messageBox.innerText = message;

    }

    function showErrorInMessageBox(message) {
        messageBox.style.backgroundColor = "#923508";
        messageBox.innerText = message;
    }


});
