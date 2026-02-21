import { defineStore } from 'pinia'

export const useAppStore = defineStore('app', {
    state: () => ({
        cart: [],
        user: null,
        tableId: null
    }),
    actions: {
        addToCart(item) {
            this.cart.push(item)
        },
        clearCart() {
            this.cart = []
        }
    }
})
