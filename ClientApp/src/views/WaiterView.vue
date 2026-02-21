<template>
  <div class="min-h-screen bg-slate-50 pb-32 lg:pb-8">
    <!-- Header -->
    <header class="bg-white border-b border-slate-200 sticky top-0 z-40 px-4 py-3 lg:px-8">
      <div class="max-w-7xl mx-auto flex justify-between items-center">
        <div class="flex items-center gap-3">
          <button @click="goBack" v-if="selectedTable" class="p-2 hover:bg-slate-100 rounded-full transition-colors lg:hidden">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-slate-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <div>
            <h1 class="text-base lg:text-xl font-black text-slate-900 leading-tight">
              {{ !selectedTable ? 'Select Table' : 'Order: #' + selectedTable.tableNumber }}
            </h1>
            <div class="flex items-center gap-2">
              <p v-if="selectedTable" class="text-[10px] uppercase tracking-widest font-bold text-slate-400">Terminal #01</p>
              <span v-if="isSyncing" class="flex items-center gap-1 text-[10px] text-orange-600 font-bold animate-pulse">
                <span class="h-1.5 w-1.5 bg-orange-600 rounded-full"></span> Syncing to Kitchen...
              </span>
              <span v-else-if="selectedTable" class="flex items-center gap-1 text-[10px] text-green-600 font-bold">
                <span class="h-1.5 w-1.5 bg-green-600 rounded-full"></span> Sent & Saved
              </span>
            </div>
          </div>
        </div>
        
        <!-- Desktop Header Info -->
        <div v-if="selectedTable" class="hidden lg:flex items-center gap-6">
           <button @click="goBack" class="text-slate-400 hover:text-slate-900 text-sm font-bold flex items-center gap-2">
             <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
               <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 15l-3-3m0 0l3-3m-3 3h8M3 12a9 9 0 1118 0 9 9 0 01-18 0z" />
             </svg>
             Change Table
           </button>
           <div class="h-10 w-px bg-slate-100"></div>
           <div class="text-right">
             <div class="text-[10px] text-slate-400 font-bold uppercase">Live Total</div>
             <div class="text-xl font-black text-orange-600 leading-none">${{ cartTotal.toFixed(2) }}</div>
           </div>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto p-4 lg:p-8">
      <!-- TABLE SELECTION -->
      <div v-if="!selectedTable" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
        <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3 lg:gap-4">
          <div 
            v-for="table in tables" 
            :key="table.id"
            @click="selectTable(table)"
            :class="[
              'p-5 lg:p-8 rounded-3xl border-2 transition-all cursor-pointer relative overflow-hidden',
              table.isOccupied 
                ? 'bg-orange-500 border-orange-600 shadow-lg shadow-orange-500/20 active:scale-95' 
                : 'bg-white border-transparent hover:border-orange-500 hover:shadow-xl hover:-translate-y-1'
            ]"
          >
            <div class="text-center relative z-10">
              <div :class="['text-2xl lg:text-3xl font-black mb-1', table.isOccupied ? 'text-white' : 'text-slate-900']">
                #{{ table.tableNumber }}
              </div>
              <div 
                :class="[
                  'text-[9px] lg:text-[10px] uppercase tracking-widest font-black px-2 py-1 rounded-full inline-block',
                  table.isOccupied ? getStatusClass(table.currentOrderStatus) : 'bg-green-50 text-green-600'
                ]"
              >
                {{ table.isOccupied ? getStatusLabel(table.currentOrderStatus) : 'Available' }}
              </div>
            </div>
            <div v-if="table.isOccupied" class="absolute -top-1 -right-1 bg-white/20 p-2 rounded-bl-2xl">
              <svg v-if="table.currentOrderStatus === 3" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-white animate-bounce" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                 <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
              </svg>
              <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- MENU & CART -->
      <div v-else class="flex flex-col lg:grid lg:grid-cols-12 gap-6 lg:gap-10 animate-in fade-in duration-500">
        <!-- Categories -->
        <div class="lg:col-span-3 sticky top-[64px] lg:top-24 bg-slate-50 lg:bg-transparent z-30 -mx-4 px-4 py-2 lg:p-0">
          <div class="flex lg:flex-col overflow-x-auto lg:overflow-x-visible gap-2 no-scrollbar">
            <button 
              @click="selectedCategory = null"
              :class="[
                'whitespace-nowrap flex-shrink-0 text-left px-5 py-3 rounded-2xl font-bold transition-all text-sm',
                selectedCategory === null ? 'bg-slate-900 text-white shadow-lg' : 'bg-white text-slate-600 border border-slate-200'
              ]"
            >
              All Items
            </button>
            <button 
              v-for="cat in categories" 
              :key="cat.id"
              @click="selectedCategory = cat.id"
              :class="[
                'whitespace-nowrap flex-shrink-0 text-left px-5 py-3 rounded-2xl font-bold transition-all text-sm',
                selectedCategory === cat.id ? 'bg-slate-900 text-white shadow-lg' : 'bg-white text-slate-600 border border-slate-200'
              ]"
            >
              {{ cat.name }}
            </button>
          </div>
        </div>

        <!-- Products Column -->
        <div class="lg:col-span-6 space-y-4">
          <div class="relative group mb-2">
            <input 
              v-model="searchQuery" 
              type="search" 
              placeholder="Search dishes..." 
              class="w-full pl-6 pr-4 py-4 bg-white border border-slate-200 rounded-2xl shadow-sm focus:ring-4 focus:ring-orange-500/10 focus:border-orange-500 outline-none font-bold text-slate-900 transition-all placeholder:text-slate-300"
            >
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div 
              v-for="product in filteredProducts" 
              :key="product.id"
              @click="addToCart(product)"
              class="bg-white rounded-2xl p-4 shadow-sm border border-slate-200 flex justify-between items-center group active:scale-95 transition-all cursor-pointer hover:border-orange-500"
            >
              <div class="flex-1 min-w-0 pr-4">
                <h4 class="font-bold text-slate-800 text-sm lg:text-base leading-tight">{{ product.name }}</h4>
                <div class="mt-1 flex items-center gap-2">
                  <span class="text-orange-600 font-black text-sm lg:text-lg">${{ product.price.toFixed(2) }}</span>
                </div>
              </div>
              <div class="h-10 w-10 bg-slate-50 text-slate-400 group-hover:bg-slate-900 group-hover:text-white rounded-xl flex items-center justify-center transition-all">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
                </svg>
              </div>
            </div>
          </div>
        </div>

        <!-- Desktop Sidebar Cart -->
        <div class="hidden lg:block lg:col-span-3">
          <div class="bg-white rounded-[32px] border border-slate-200 p-6 sticky top-24 max-h-[calc(100vh-120px)] flex flex-col shadow-sm">
             <div class="flex justify-between items-center mb-6">
                <h2 class="text-lg font-black text-slate-900">Order List</h2>
                <div class="p-2 hover:bg-slate-50 rounded-xl cursor-default transition-colors">
                   <svg v-if="historyLoading" class="animate-spin h-5 w-5 text-slate-300" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                     <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                     <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                   </svg>
                </div>
             </div>

             <div class="flex-1 overflow-y-auto space-y-3 no-scrollbar">
                <div v-if="cart.length === 0" class="text-center py-20 flex flex-col items-center gap-3">
                   <div class="h-16 w-16 bg-slate-50 rounded-full flex items-center justify-center text-slate-200">
                     <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                       <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                     </svg>
                   </div>
                   <p class="text-slate-300 text-xs font-bold uppercase tracking-widest">No Items</p>
                </div>
                <div v-for="item in cart" :key="item.id" class="bg-slate-50 p-3 rounded-2xl border border-slate-100 group animate-in slide-in-from-right-2">
                   <div class="flex-1 min-w-0">
                     <div class="flex justify-between items-start">
                       <h4 class="font-bold text-slate-800 text-sm truncate leading-tight">{{ item.name }}</h4>
                       <button @click="removeFromCart(item)" class="text-slate-300 hover:text-red-500 transition-colors">
                         <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                           <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                         </svg>
                       </button>
                     </div>
                     <div class="flex items-center justify-between mt-2">
                       <div class="text-xs font-bold text-orange-600">${{ (item.price * item.quantity).toFixed(2) }}</div>
                       <div class="flex items-center gap-2 bg-slate-100 rounded-lg p-1">
                         <button @click="updateQuantity(item, -1)" class="h-6 w-6 flex items-center justify-center hover:bg-white rounded shadow-sm transition-colors text-slate-600">-</button>
                         <span class="text-xs font-bold w-4 text-center">{{ item.quantity }}</span>
                         <button @click="updateQuantity(item, 1)" class="h-6 w-6 flex items-center justify-center hover:bg-white rounded shadow-sm transition-colors text-slate-600">+</button>
                       </div>
                     </div>
                     <!-- Special Instructions -->
                     <div class="mt-2">
                       <input 
                         type="text" 
                         v-model="item.specialInstructions"
                         placeholder="Special instructions (e.g. No onion)"
                         class="w-full text-[10px] bg-slate-50 border border-slate-200 rounded px-2 py-1 focus:outline-none focus:border-orange-300"
                       />
                     </div>
                   </div>
                </div>
             </div>
             
             <div class="mt-6 pt-4 border-t border-slate-100 flex justify-between items-center">
                <span class="text-slate-400 font-bold text-xs uppercase tracking-widest">Table Total</span>
                <span class="text-xl font-black text-slate-900">${{ cartTotal.toFixed(2) }}</span>
             </div>
          </div>
        </div>
      </div>

    <!-- MOBILE CART DRAWER (Bottom Bar) -->
    <div v-if="selectedTable" class="lg:hidden fixed bottom-6 left-4 right-4 z-50">
      <div 
        @click="isCartOpen = !isCartOpen"
        class="bg-slate-900 rounded-[28px] p-4 flex items-center justify-between shadow-2xl relative overflow-hidden"
      >
         <div class="flex items-center gap-4">
            <div class="h-10 w-10 bg-white/10 rounded-xl flex items-center justify-center">
               <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                 <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
               </svg>
            </div>
            <div>
               <div class="text-[8px] font-black text-slate-400 uppercase tracking-widest leading-none mb-1">Total Order</div>
               <div class="text-lg font-black text-orange-500 leading-none">${{ cartTotal.toFixed(2) }}</div>
            </div>
         </div>
         
         <div class="flex items-center gap-3">
            <span v-if="isSyncing" class="h-5 w-5 border-2 border-white/20 border-t-orange-500 rounded-full animate-spin"></span>
            <span class="bg-white/10 text-white text-[10px] font-bold px-3 py-1.5 rounded-full">
              {{ cartCount }} Items
            </span>
            <svg :class="['h-5 w-5 text-slate-400 transition-transform', isCartOpen ? 'rotate-180' : '']" fill="none" viewBox="0 0 24 24" stroke="currentColor">
               <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
            </svg>
         </div>
      </div>

      <!-- Mobile Expandable Content -->
      <div v-if="isCartOpen" class="absolute bottom-[80px] left-0 right-0 bg-white rounded-[32px] shadow-2xl border border-slate-200 p-6 max-h-[50vh] overflow-y-auto no-scrollbar animate-in slide-in-from-bottom-5">
         <div class="flex justify-between items-center mb-6">
            <h3 class="font-black text-slate-900">Items Ordered</h3>
            <button @click.stop="isCartOpen = false" class="text-slate-400 text-xs font-bold p-2">Hide</button>
         </div>
         <div class="space-y-4">
            <div v-if="cart.length === 0" class="text-center py-10 text-slate-300 text-sm">Tap items to add</div>
            <div v-for="item in cart" :key="item.id" class="flex items-center justify-between pb-4 border-b border-slate-50 last:border-0 last:pb-0">
               <div class="flex-1 pr-4">
                  <div class="font-bold text-slate-900 text-sm leading-tight">{{ item.name }}</div>
                  <div class="text-orange-600 font-black text-xs mt-1 text-sm">${{ (item.price * item.quantity).toFixed(2) }}</div>
               </div>
               <div class="flex items-center gap-3">
                  <div class="flex items-center bg-slate-100 rounded-xl p-0.5">
                     <button @click.stop="updateQuantity(item, -1)" class="h-8 w-8 text-slate-400 font-bold text-lg">-</button>
                     <span class="w-6 text-center font-black text-slate-900 text-xs">{{ item.quantity }}</span>
                     <button @click.stop="updateQuantity(item, 1)" class="h-8 w-8 text-slate-400 font-bold text-lg">+</button>
                  </div>
                  <button @click.stop="removeFromCart(item)" class="p-2 text-red-500 bg-red-50 rounded-lg">
                     <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                     </svg>
                  </button>
               </div>
            </div>
         </div>
      </div>
    </div>
  </main>
</div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'

// State
const tables = ref([])
const products = ref([])
const categories = ref([])
const selectedTable = ref(null)
const selectedCategory = ref(null)
const searchQuery = ref('')
const cart = ref([])
const isSyncing = ref(false)
const isCartOpen = ref(false)
const historyLoading = ref(false)
const isNavigating = ref(false)
let syncTimeout = null

// Computed
const filteredProducts = computed(() => {
  let filtered = products.value
  if (selectedCategory.value) filtered = filtered.filter(p => p.categoryId === selectedCategory.value)
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    filtered = filtered.filter(p => p.name.toLowerCase().includes(q))
  }
  return filtered
})

const cartCount = computed(() => cart.value.reduce((sum, item) => sum + item.quantity, 0))
const cartTotal = computed(() => cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0))

// Sync Logic
const syncWithBackend = async () => {
  if (!selectedTable.value || isNavigating.value) return
  isSyncing.value = true
  
  try {
    const res = await fetch('/api/orders/sync', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        tableId: selectedTable.value.id,
        orderItems: cart.value.map(i => ({
          productId: i.id,
          quantity: i.quantity,
          unitPrice: i.price,
          specialInstructions: i.specialInstructions
        }))
      })
    })
    
    if (!res.ok) console.error('Sync failed')
  } catch (e) {
    console.error('Network error during sync')
  } finally {
    isSyncing.value = false
  }
}

// Watch cart for automatic syncing (with debounce)
watch(cart, () => {
  if (syncTimeout) clearTimeout(syncTimeout)
  syncTimeout = setTimeout(syncWithBackend, 800) // Small delay to batch multiple taps
}, { deep: true })

// Methods
const fetchData = async () => {
  try {
    const t = Date.now()
    const [tRes, pRes, cRes, oRes] = await Promise.all([
      fetch(`/api/tables?t=${t}`),
      fetch(`/api/products?t=${t}`),
      fetch(`/api/categories?t=${t}`),
      fetch(`/api/orders/pending?t=${t}`)
    ])
    if (tRes.ok) tables.value = await tRes.json()
    if (pRes.ok) products.value = await pRes.json()
    if (cRes.ok) categories.value = await cRes.json()
    
    // Enrich tables with status
    if (oRes.ok) {
       const pending = await oRes.json()
       tables.value = tables.value.map(table => {
         const order = pending.find(o => o.tableId === table.id)
         return {
           ...table,
           currentOrderStatus: order ? order.status : 0
         }
       })
    }
  } catch (e) {
    console.error('Fetch error:', e)
  }
}

const getStatusLabel = (status) => {
  switch(status) {
    case 2: return 'Preparing'
    case 3: return 'Ready (Serve!)'
    default: return 'Occupied'
  }
}

const getStatusClass = (status) => {
  switch(status) {
    case 2: return 'bg-blue-600 text-blue-100 animate-pulse'
    case 3: return 'bg-green-600 text-green-100 animate-bounce'
    default: return 'bg-orange-600 text-orange-200'
  }
}

const fetchCurrentOrder = async (tableId) => {
  historyLoading.value = true
  try {
    const res = await fetch(`/api/orders/pending?t=${Date.now()}`)
    if (res.ok) {
       const pending = await res.json()
        // Note: GetPendingOrders API already filters out Paid/Cancelled, 
        // but we find the first match for this table among all active ones.
        const currentOrder = pending.find(o => o.tableId === tableId) 
        if (currentOrder && currentOrder.orderItems) {
        // Populate cart
        cart.value = currentOrder.orderItems.map(oi => ({
          id: oi.productId,
          name: oi.product.name,
          price: oi.unitPrice,
          quantity: oi.quantity,
          specialInstructions: oi.specialInstructions || ''
        }))
          return true
       }
    }
    return false
  } finally {
    historyLoading.value = false
  }
}

const goBack = async () => {
  // Prevent sync when clearing cart for navigation
  isNavigating.value = true
  
  // Final sync before exiting if there were pending changes
  if (syncTimeout) {
    clearTimeout(syncTimeout)
    // Only sync if we have items, otherwise it's just a view switch
    if (cart.value.length > 0) {
      await syncWithBackend()
    }
  }
  
  selectedTable.value = null
  cart.value = [] // Clear items so next table starts fresh
  searchQuery.value = ''
  isCartOpen.value = false
  
  // Reset navigation flag after state is cleared
  setTimeout(() => {
    isNavigating.value = false
  }, 100)
  
  fetchData() 
}

const selectTable = async (table) => {
  if (table.isOccupied) {
    // If loading an occupied table, we MUST overwrite the cart
    const loaded = await fetchCurrentOrder(table.id)
    if (!loaded) cart.value = [] // Safety: if occupied but no pending items found
  }
  // If NOT occupied, we keep current cart (allows 'Carry to Table' workflow)
  selectedTable.value = table
}

const addToCart = (product) => {
  const existing = cart.value.find(i => i.id === product.id)
  if (existing) existing.quantity++
  else  cart.value.push({ ...product, quantity: 1, specialInstructions: '' })
}

const updateQuantity = (item, delta) => {
  item.quantity += delta
  if (item.quantity <= 0) {
    removeFromCart(item)
  }
}

const removeFromCart = (item) => {
  cart.value = cart.value.filter(i => i.id !== item.id)
}

onMounted(fetchData)
</script>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
