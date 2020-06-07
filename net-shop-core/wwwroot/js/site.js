﻿
// Write your JavaScript code.
$(document).ready(function () {

    /**
    * Sets modal image for product
    *
    */
    $(".pr-main-img").click(function () {
        const img_link = $(this).attr('src');
        $("#modalImage").attr("src", img_link);
        $("#modalImageLink").attr("href", img_link);
        $('#imageModal').modal('show');
    });


    /**
    * Sets main product image to clicked image
    *
    */
    $(".pr-sub-img").click(function () {
        const host = window.location.origin;
        const img_link = host+"/"+$(this).attr('src');
        $(".pr-main-img").attr("src", img_link);
    });

    
    /**
    * Adds product data to shopping cart session data
    *
    * Get product data from add to cart button's data-*
    * 
    * Seperates different products by [#]
    * 
    */
    $(".cart-btn").click(function () {

        const product_id = $(this).data("product-id");
        const product_name = $(this).data("product-name");
        const product_price = $(this).data("product-price");
        const product_quantity = $(this).data("product-quantity");
        const product_image = $(this).data("product-image");
        const session_data = product_id + "," + product_name + "," + product_price + "," + product_quantity + "," + product_image + "[#]";
        const current_session = $.session.get('ShoppingCart');
        $.session.set('ShoppingCart', current_session + session_data);

        setCartNumber();//update total number in cart 


        //$.session.clear();
        //const cart_data = formatCartSession();
        //$.each(cart_data.split("[#]").slice(0, -1), function (index, item) {
        //    var data_array = item.split(',');
        //    alert("PID: " + data_array[0] + "| PNM: " + data_array[1] + "| PPR: " + data_array[2] + "| PQN: " + data_array[3] + "| PIM: " + data_array[4]);
        //});
    });


    /**
    * Updates the total number of product items in  cart
    * 
    */
    setTimeout(
    function () {
        //wait 0.01 seconds and update total number in cart 
        setCartNumber();
    }, 10);

    
    /**
    * Updates the number of product quantity in data-quantity
    *
    * Disables cart button if product number is less than 0
    */
    $(".update-quantity").click(function () {
        setTimeout(
        function () {
            //wait 0.2 seconds for element to be updated
            const total_quantity = $("#total-quantity").val(); //get current quantity
            $('.cart-btn').data('product-quantity', total_quantity);

            //if quantity is 0, disable add to cart button
            if (parseInt(total_quantity) < 1) {
                $(".cart-btn").addClass(" disabled");
            }
            else {
                $(".cart-btn").removeClass("disabled");
            }

        }, 200);
    });


    /**
    * Clears shopping cart, then closes confirmation modal
    *
    */
    $("#ClearCart").click(function () {
        setTimeout(
        function () {
            $.session.set('ShoppingCart', "");
                $('#clearCartModal').modal('hide');
                location.reload();// reload page
        }, 100);
    });


    /**
    * Checks if strings has undefined, replaces it with empty
    *
    * @return
    *   The value of the current session, null or string without 'undefined'.
    */
    function formatCartSession() {
        var cart_session = $.session.get('ShoppingCart');
        if (cart_session != "" && cart_session != null) {
            if (cart_session.length > 8 && cart_session.indexOf("undefined") >= 0) { 
                return cart_session.replace('undefined', '');
            }
            return cart_session;
        }

        return null;
    }

    /**
    * Updates the total number of item in cart
    *
    */
    function setCartNumber() {
        const total_num = getTotalInCart();
        $(".shopping-cart-count").text(total_num);
    }


    /**
    * Counts the number of item in cart
    *
    * @return
    *   The to total number of items in the shopping cart
    */
    function getTotalInCart() {
        var cart_data = $.session.get('ShoppingCart');
        var total_num = 0;
        if (cart_data != null) {
            $.each(cart_data.split("[#]").slice(0, -1), function (index, item) {
                total_num++;
            });
        }
        return total_num;
    }


    /**
    * Generates shopping cart table if in shopping cart checkout page
    *
    */
    setTimeout(
        function () {
            const host = window.location.origin;//get site host url
            const cart_data = formatCartSession(); //get current cart data
            $(".other-cart-components").show();//show Update, Shopping, and Coupon components

            //If in shopping cart page
            if ($('#ShoppingCartDiv').length && cart_data != null) {
                var content = "<table class='table table-bordered'>"
                content += `<thead>
	                        <tr>
		                        <th class='product-thumbnail'>Image</th>
		                        <th class='product-name'>Product</th>
		                        <th class='product-price'>Price</th>
		                        <th class='product-quantity'>Quantity</th>
		                        <th class='product-total'>Total</th>
		                        <th class='product-remove'>Remove</th>
	                        </tr>
                        </thead>`
                content += "<tbody>"
                //generate cart data here
                $.each(cart_data.split("[#]").slice(0, -1), function (index, item) {
                    var data_array = item.split(',');
                    content += `<tr> 
	                            <td class='product-thumbnail'>
		                            <img src='`+ host + `/` + data_array[4] + `' alt='Image' class='rounded' height='162' width='115'>
	                            </td>
	                            <td class='product-name'>
		                            <h2 class='h5 text-black'>` + data_array[1] + `</h2>
	                            </td>
	                            <td>` + data_array[2] + `</td>
	                            <td>
		                            <div class='input-group mb-3' style='max-width: 120px;'>
			                            <div class='input-group-prepend'>
				                            <button class='btn btn-outline-primary js-btn-minus' type='button'>&minus;</button>
			                            </div>
			                            <input type='text' class='form-control text-center' value='` + data_array[3] + `' placeholder='' aria-label='Example text with button addon' aria-describedby='button-addon1'>
			                            <div class='input-group-append'>
				                            <button class='btn btn-outline-primary js-btn-plus' type='button'>&plus;</button>
			                            </div>
		                            </div>

	                            </td>
	                            <td>` + data_array[2] * data_array[3] + `</td>
	                            <td><a href='#' class='btn btn-primary btn-sm'>X</a></td>
                            </tr>`
                });
                content += "</tbody>"
                content += "</table>"

                $('#DynamicCartTable').append(content);
            }
            else {
                $('#EmptyCartText').text("Cart is empty");
                $(".other-cart-components").hide();//hide Update, Shopping, and Coupon components
            }
    }, 100);



});