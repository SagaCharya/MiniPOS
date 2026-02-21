<template>
  <div class="min-h-screen">
    <!-- Header -->
    <header class="bg-slate-900 text-white p-6 shadow-lg">
      <div class="max-w-7xl mx-auto flex items-center gap-4">
        <button @click="$router.push('/')" class="hover:bg-slate-800 p-2 rounded-full transition-colors">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z" clip-rule="evenodd" />
          </svg>
        </button>
        <div>
          <h1 class="text-2xl font-bold">Admin Dashboard</h1>
          <p class="text-slate-400 text-sm">Manage products and view sales</p>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto p-6 md:p-8">
      <!-- Tabs -->
      <div class="flex gap-2 mb-8 bg-slate-200/50 p-1 rounded-xl w-fit">
        <button 
          @click="activeTab = 'analytics'"
          :class="[
            'px-8 py-2.5 rounded-lg font-medium transition-all duration-200',
            activeTab === 'analytics' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-500 hover:text-slate-700'
          ]"
        >
          Sales Analytics
        </button>
        <button 
          @click="activeTab = 'billing'"
          :class="[
            'px-8 py-2.5 rounded-lg font-medium transition-all duration-200',
            activeTab === 'billing' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-500 hover:text-slate-700'
          ]"
        >
          Counter Bill
        </button>
        <button 
          @click="activeTab = 'products'"
          :class="[
            'px-8 py-2.5 rounded-lg font-medium transition-all duration-200',
            activeTab === 'products' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-500 hover:text-slate-700'
          ]"
        >
          Products
        </button>
        <button 
          @click="activeTab = 'tables'"
          :class="[
            'px-8 py-2.5 rounded-lg font-medium transition-all duration-200',
            activeTab === 'tables' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-500 hover:text-slate-700'
          ]"
        >
          Tables
        </button>
      </div>

      <!-- Analytics Tab Content -->
      <div v-if="activeTab === 'analytics'" class="space-y-8 animate-in fade-in duration-500">
        <!-- Stats Cards -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div v-for="stat in statsCards" :key="stat.label" class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 hover:shadow-md transition-shadow">
            <div class="flex justify-between items-start mb-4">
              <span class="text-slate-500 font-medium">{{ stat.label }}</span>
              <div :class="[stat.iconBg, 'p-2 rounded-xl text-white']">
                <component :is="stat.icon" class="h-5 w-5" />
              </div>
            </div>
            <div class="text-3xl font-bold mb-1">${{ stat.value }}</div>
            <div class="text-slate-400 text-sm">{{ stat.subtext }}</div>
          </div>
        </div>

        <!-- Top Selling Items -->
        <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6">
          <h2 class="text-lg font-bold mb-6">Top Selling Items</h2>
          <div class="space-y-4">
            <div v-for="(item, index) in topItems" :key="item.name" class="flex items-center justify-between p-4 bg-slate-50 rounded-xl">
              <div class="flex items-center gap-4">
                <span class="flex items-center justify-center h-8 w-8 rounded-full bg-orange-100 text-orange-700 font-bold text-sm">
                  {{ index + 1 }}
                </span>
                <div>
                  <div class="font-bold text-slate-900">{{ item.name }}</div>
                  <div class="text-slate-500 text-sm">{{ item.sold }} sold</div>
                </div>
              </div>
              <div class="text-orange-600 font-bold">${{ (item.revenue || 0).toFixed(2) }}</div>
            </div>
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="bg-white rounded-2xl shadow-sm border border-slate-100 p-6">
          <h2 class="text-lg font-bold mb-6">Recent Orders</h2>
          <div class="divide-y divide-slate-100">
            <div v-for="order in recentOrders" :key="order.id" class="py-4 first:pt-0 last:pb-0">
              <div class="flex justify-between items-center">
                <div>
                  <div class="font-bold text-slate-900">{{ order.tableName }}</div>
                  <div class="text-slate-400 text-xs mt-1">
                    {{ formatDate(order.orderDate) }} • {{ order.itemCount }} items
                  </div>
                </div>
                <div class="text-right">
                  <div class="text-orange-600 font-bold">${{ (order.totalAmount || 0).toFixed(2) }}</div>
                  <div class="text-slate-400 text-xs mt-1 uppercase tracking-wider">{{ order.status }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Payment Breakdown -->
        <div class="mt-8 pt-8 border-t border-slate-100">
          <h3 class="text-xs font-black uppercase tracking-widest text-slate-400 mb-6 text-center">Revenue by Channel</h3>
          <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
            <div v-for="method in dashboardData.salesByMethod" :key="method.method" class="p-4 bg-slate-50 rounded-2xl border border-slate-100">
               <div class="text-[10px] font-black uppercase tracking-widest text-slate-400 mb-1">{{ method.method }}</div>
               <div class="text-lg font-black text-slate-900">${{ method.amount.toFixed(2) }}</div>
               <div class="text-[10px] text-slate-500 font-medium">{{ method.count }} Orders</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Billing Tab Content -->
      <div v-else-if="activeTab === 'billing'" class="animate-in fade-in duration-500">
        <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <!-- Left: Order List -->
          <div class="lg:col-span-4 space-y-4">
            <div class="bg-white p-4 rounded-2xl shadow-sm border border-slate-100 mb-6">
              <div class="relative">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input v-model="billingSearch" type="text" placeholder="Search by table number..." class="w-full pl-10 pr-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:ring-2 focus:ring-orange-500/20 outline-none transition-all">
              </div>
            </div>

            <div class="space-y-3 overflow-y-auto max-h-[calc(100vh-350px)] pr-2 custom-scrollbar">
              <div 
                v-for="order in filteredOrders" 
                :key="order.id" 
                @click="selectBill(order)"
                :class="[
                  'p-4 rounded-2xl cursor-pointer transition-all border-2',
                  selectedBill?.id === order.id ? 'bg-white border-orange-500 shadow-md' : 'bg-slate-50 border-transparent hover:bg-slate-100'
                ]"
              >
                <div class="flex justify-between items-start mb-2">
                  <div>
                    <div class="font-bold text-slate-900">Order #ORD-{{ order.id }}</div>
                    <div class="text-slate-500 text-sm">Table {{ order.table?.tableNumber || 'N/A' }}</div>
                  </div>
                  <div class="text-orange-600 font-bold">${{ order.totalAmount.toFixed(2) }}</div>
                </div>
                <div class="flex items-center justify-between mt-4">
                   <div class="text-[10px] text-slate-400 font-medium uppercase">{{ formatDateShort(order.orderDate) }}</div>
                   <span :class="['px-2 py-0.5 rounded text-[9px] font-bold uppercase transition-all duration-300', order.status === 'Paid' ? 'bg-green-100 text-green-700' : 'bg-orange-100 text-orange-700']">{{ order.status }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Right: Bill Details -->
          <div class="lg:col-span-8">
            <div v-if="selectedBill" class="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
              <div class="p-8">
                 <div class="flex justify-between items-start mb-8">
                    <div>
                      <h2 class="text-2xl font-bold text-slate-900">Bill Details</h2>
                      <div class="text-slate-400 mt-1">Order ID #ORD-{{ selectedBill.id }}</div>
                    </div>
                    <span class="px-3 py-1 bg-slate-900 text-white rounded-lg text-xs font-bold uppercase tracking-wider">{{ selectedBill.status }}</span>
                 </div>

                 <div class="grid grid-cols-2 gap-8 mb-8 pb-8 border-b border-slate-100">
                    <div>
                      <div class="text-slate-400 text-xs font-bold uppercase tracking-widest mb-1">Table Number</div>
                      <div class="text-xl font-bold text-slate-900">{{ selectedBill.table?.tableNumber || 'Takeaway' }}</div>
                    </div>
                    <div>
                      <div class="text-slate-400 text-xs font-bold uppercase tracking-widest mb-1">Order Time</div>
                      <div class="text-sm font-medium text-slate-600">{{ formatDate(selectedBill.orderDate) }}</div>
                    </div>
                 </div>

                 <div class="space-y-6 mb-8">
                    <div class="flex justify-between items-center">
                      <h3 class="font-bold text-slate-900">Items</h3>
                      <button @click="openAddProductToBill" class="text-orange-600 text-xs font-bold flex items-center gap-1 hover:text-orange-700">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                          <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
                        </svg>
                        Add Item
                      </button>
                    </div>
                    <div class="space-y-4">
                      <div v-for="item in selectedBill.orderItems" :key="item.id" class="flex justify-between items-center text-sm group py-1">
                        <div class="text-slate-600 flex items-center gap-3">
                          <button @click="removeItemFromBill(item)" class="text-slate-300 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-all">
                             <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                             </svg>
                          </button>
                          
                          <div class="flex items-center gap-2 bg-slate-50 rounded-lg p-0.5 border border-slate-100">
                            <button @click="updateItemQty(item, -1)" class="h-5 w-5 flex items-center justify-center hover:bg-white hover:shadow-sm rounded transition-all text-slate-400 hover:text-orange-600 font-bold">-</button>
                            <span class="font-bold text-slate-900 min-w-[1.25rem] text-center">{{ item.quantity }}</span>
                            <button @click="updateItemQty(item, 1)" class="h-5 w-5 flex items-center justify-center hover:bg-white hover:shadow-sm rounded transition-all text-slate-400 hover:text-orange-600 font-bold">+</button>
                          </div>

                          <span class="font-medium text-slate-700">{{ item.product?.name }}</span>
                        </div>
                        <div class="font-bold text-slate-900">${{ (item.quantity * item.unitPrice).toFixed(2) }}</div>
                      </div>
                    </div>
                 </div>

                 <div class="space-y-4 mb-8 pt-8 border-t border-slate-100">
                    <div class="grid grid-cols-2 gap-4">
                      <div>
                        <label class="block text-xs font-bold text-slate-400 uppercase tracking-widest mb-2">Discount (%)</label>
                        <input v-model.number="selectedBill.discountPercentage" type="number" class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:ring-2 focus:ring-orange-500/20 outline-none font-bold text-slate-900 transition-all">
                      </div>
                      <div>
                        <label class="block text-xs font-bold text-slate-400 uppercase tracking-widest mb-2">Tax (%)</label>
                        <input v-model.number="selectedBill.taxPercentage" type="number" class="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:ring-2 focus:ring-orange-500/20 outline-none font-bold text-slate-900 transition-all">
                      </div>
                    </div>
                 </div>

                  <!-- Payment Method Selection -->
                  <div class="mt-8 mb-6 pt-6 border-t border-slate-100">
                    <label class="block text-xs font-bold text-slate-400 uppercase tracking-widest mb-4 text-center">Payment Method</label>
                    <div class="grid grid-cols-3 gap-3">
                      <button 
                        v-for="method in ['Cash', 'Card', 'QR']" 
                        :key="method"
                        @click="selectedBill.paymentMethod = method"
                        :class="[
                          'py-3 rounded-xl border-2 font-bold transition-all text-sm',
                          selectedBill.paymentMethod === method ? 'bg-orange-500 border-orange-500 text-white shadow-lg shadow-orange-500/20' : 'bg-white border-slate-100 text-slate-500 hover:border-slate-200'
                        ]"
                      >
                        {{ method }}
                      </button>
                    </div>
                  </div>

                 <div class="bg-slate-50 rounded-2xl p-6 space-y-3">
                    <div class="flex justify-between text-slate-500 text-sm">
                      <span>Subtotal</span>
                      <span class="font-bold text-slate-900">${{ billSubtotal.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between text-slate-500 text-sm">
                      <span>Discount ({{ selectedBill.discountPercentage }}%)</span>
                      <span class="font-bold text-red-500">-${{ billDiscountAmount.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between text-slate-500 text-sm">
                      <span>Tax ({{ selectedBill.taxPercentage }}%)</span>
                      <span class="font-bold text-slate-900">+${{ billTaxAmount.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between items-center pt-4 border-t border-slate-200">
                      <span class="text-lg font-bold text-slate-900">Total</span>
                      <span class="text-2xl font-black text-orange-600">${{ billTotal.toFixed(2) }}</span>
                    </div>
                 </div>

                 <div class="mt-8 flex gap-3">
                    <button 
                      @click="showReceiptModal = true"
                      class="flex-1 py-4 bg-slate-100 text-slate-900 rounded-2xl font-bold hover:bg-slate-200 transition-all text-sm"
                    >
                      Print Receipt
                    </button>
                    <button 
                      @click="settleBill"
                      :disabled="isSettling || !selectedBill.paymentMethod"
                      class="flex-[2] bg-slate-900 text-white py-4 rounded-2xl font-bold hover:bg-slate-800 transition-all flex items-center justify-center gap-2 shadow-xl shadow-slate-900/10 active:scale-[0.98] disabled:opacity-50"
                    >
                      <span v-if="isSettling" class="h-5 w-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                      {{ isSettling ? 'Processing...' : 'Paid & Settle' }}
                    </button>
                 </div>
              </div>
            </div>
            <div v-else class="h-full flex flex-col items-center justify-center text-slate-400 bg-white rounded-3xl border border-dashed border-slate-200 p-12">
               <div class="p-6 bg-slate-50 rounded-full mb-4">
                 <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                   <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                 </svg>
               </div>
               <p class="font-bold text-slate-900">No bill selected</p>
               <p class="text-sm">Select an order from the list to manage billing</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Tables Tab Content -->
      <div v-else-if="activeTab === 'tables'" class="animate-in fade-in duration-500">
        <div class="flex justify-between items-center mb-8">
          <h2 class="text-2xl font-bold">Dining Tables</h2>
          <button @click="openAddTable" class="bg-slate-900 text-white px-6 py-2.5 rounded-xl font-bold hover:bg-slate-800 transition-all flex items-center gap-2 shadow-lg shadow-slate-900/10">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
            </svg>
            Add Table
          </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
          <div v-for="table in tables" :key="table.id" class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 hover:shadow-md transition-all">
            <div class="flex justify-between items-start mb-4">
              <div class="p-3 bg-orange-100/10 text-orange-600 rounded-xl">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                </svg>
              </div>
              <span :class="[
                'px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider',
                table.isOccupied ? 'bg-orange-100 text-orange-700' : 'bg-green-100 text-green-700'
              ]">
                {{ table.isOccupied ? 'Occupied' : 'Available' }}
              </span>
            </div>
            <h3 class="text-xl font-bold text-slate-900 mb-1">Table {{ table.tableNumber }}</h3>
            <p class="text-slate-500 text-sm mb-6">Capacity: {{ table.capacity }} People</p>
            
            <div class="flex gap-2">
              <button @click="editTable(table)" class="flex-1 py-2 text-sm font-medium text-slate-700 border border-slate-200 rounded-xl hover:bg-slate-50 transition-colors">Edit</button>
              <button @click="confirmDeleteTable(table)" class="flex-1 py-2 text-sm font-medium text-red-600 bg-red-50 rounded-xl hover:bg-red-100 transition-colors">Delete</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Products Tab Content -->
      <div v-else class="animate-in fade-in duration-500">
        <div class="flex justify-between items-center mb-8">
          <h2 class="text-2xl font-bold">Menu Items</h2>
          <div class="flex gap-4">
            <button @click="openAddCategory" class="bg-slate-900 text-white px-6 py-2.5 rounded-xl font-bold hover:bg-slate-800 transition-all flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
              </svg>
              Add Category
            </button>
            <button @click="openAddProduct" class="bg-orange-600 hover:bg-orange-700 text-white px-6 py-2.5 rounded-xl font-bold shadow-lg shadow-orange-600/20 transition-all flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
              </svg>
              Add Product
            </button>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div v-for="product in products" :key="product.id" class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden hover:shadow-lg transition-all group">
            <div class="h-48 bg-slate-100 relative overflow-hidden">
              <img :src="product.imageUrl || '/placeholder-food.jpg'" :alt="product.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
              <div class="absolute top-4 right-4 bg-white/90 backdrop-blur px-3 py-1 rounded-lg text-xs font-bold text-slate-900">
                {{ product.category?.name || 'Main Course' }}
              </div>
            </div>
            <div class="p-6">
              <div class="flex justify-between items-start mb-2">
                <h3 class="font-bold text-lg text-slate-900">{{ product.name }}</h3>
                <span class="text-orange-600 font-bold">${{ (product.price || 0).toFixed(2) }}</span>
              </div>
              <p class="text-slate-500 text-sm mb-6 line-clamp-2">{{ product.description }}</p>
              
              <div class="grid grid-cols-2 gap-3">
                <button @click="editProduct(product)" class="flex items-center justify-center gap-2 py-2 border border-slate-200 rounded-xl hover:bg-slate-50 transition-colors font-medium text-slate-700">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                  </svg>
                  Edit
                </button>
                <button @click="confirmDelete(product)" class="flex items-center justify-center gap-2 py-2 bg-red-50 text-red-600 rounded-xl hover:bg-red-100 transition-colors font-medium">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd" />
                  </svg>
                  Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Add/Edit Product Modal -->
    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="closeModal"></div>
      <div class="bg-white rounded-3xl shadow-2xl w-full max-w-lg relative z-10 overflow-hidden animate-in zoom-in duration-300">
        <div class="p-8">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-slate-900">{{ isEditing ? 'Edit Product' : 'Add New Product' }}</h2>
            <button @click="closeModal" class="text-slate-400 hover:text-slate-600 transition-colors">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="saveProduct" class="space-y-5">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Product Name</label>
              <input v-model="form.name" type="text" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none" placeholder="e.g. Margherita Pizza">
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-bold text-slate-700 mb-2">Price ($)</label>
                <input v-model.number="form.price" type="number" step="0.01" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none" placeholder="0.00">
              </div>
              <div>
                <label class="block text-sm font-bold text-slate-700 mb-2">Category</label>
                <select v-model="form.categoryId" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none appearance-none">
                  <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Description</label>
              <textarea v-model="form.description" rows="3" class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none resize-none" placeholder="Brief description of the dish..."></textarea>
            </div>

            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Product Image</label>
              <div class="flex gap-4 items-center">
                <div class="h-16 w-16 bg-slate-100 rounded-xl overflow-hidden flex-shrink-0 border border-slate-200">
                  <img v-if="form.imageUrl" :src="form.imageUrl" class="h-full w-full object-cover">
                  <div v-else class="h-full w-full flex items-center justify-center text-slate-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                  </div>
                </div>
                <div class="flex-1">
                  <input type="file" @change="handleFileUpload" accept="image/*" class="hidden" ref="fileInput">
                  <button type="button" @click="$refs.fileInput.click()" class="w-full px-4 py-3 bg-white border-2 border-dashed border-slate-200 rounded-xl text-slate-600 font-medium hover:border-orange-500 hover:text-orange-600 transition-all text-sm">
                    Choose from PC
                  </button>
                  <p class="text-[10px] text-slate-400 mt-1">Or paste a URL below</p>
                </div>
              </div>
              <input v-model="form.imageUrl" type="text" class="w-full px-4 py-3 mt-2 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none" placeholder="https://example.com/image.jpg or /uploads/...">
            </div>

            <div class="pt-4">
              <button type="submit" :disabled="isSaving" class="w-full bg-slate-900 text-white py-4 rounded-xl font-bold hover:bg-slate-800 transition-all flex items-center justify-center gap-2 shadow-xl shadow-slate-900/10 active:scale-[0.98]">
                <span v-if="isSaving" class="h-5 w-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                {{ isSaving ? 'Saving...' : (isEditing ? 'Update Product' : 'Create Product') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Category Modal -->
    <div v-if="showCategoryModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="closeCategoryModal"></div>
      <div class="bg-white rounded-3xl shadow-2xl w-full max-w-sm relative z-10 overflow-hidden animate-in zoom-in duration-300">
        <div class="p-8">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-slate-900">New Category</h2>
            <button @click="closeCategoryModal" class="text-slate-400 hover:text-slate-600 transition-colors">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="saveCategory" class="space-y-5">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Category Name</label>
              <input v-model="categoryName" type="text" required class="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500/20 focus:border-orange-500 transition-all outline-none" placeholder="e.g. Seafood">
            </div>

            <div class="pt-4">
              <button type="submit" :disabled="isSavingCategory" class="w-full bg-slate-900 text-white py-4 rounded-xl font-bold hover:bg-slate-800 transition-all flex items-center justify-center gap-2 shadow-xl shadow-slate-900/10">
                {{ isSavingCategory ? 'Saving...' : 'Create Category' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Add Product to Bill Modal -->
    <div v-if="showAddProductToBillModal" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="showAddProductToBillModal = false"></div>
      <div class="bg-white rounded-3xl shadow-2xl w-full max-w-2xl relative z-10 overflow-hidden animate-in zoom-in duration-300">
        <div class="p-8">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold text-slate-900">Add Item to Bill</h2>
            <button @click="showAddProductToBillModal = false" class="text-slate-400 hover:text-slate-600 transition-colors">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <div class="mb-6">
            <div class="relative">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input v-model="productToBillSearch" type="text" placeholder="Search products..." class="w-full pl-10 pr-4 py-3 bg-slate-50 border border-slate-100 rounded-xl focus:ring-2 focus:ring-orange-500/20 outline-none transition-all">
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 max-h-[400px] overflow-y-auto pr-2 custom-scrollbar">
            <div 
              v-for="product in filteredProductsToBill" 
              :key="product.id"
              @click="addItemToBill(product)"
              class="p-4 bg-slate-50 border border-transparent hover:border-orange-500 hover:bg-white rounded-2xl cursor-pointer transition-all flex items-center gap-4"
            >
              <div class="h-12 w-12 bg-slate-200 rounded-xl overflow-hidden flex-shrink-0">
                <img :src="product.imageUrl || '/placeholder-food.jpg'" class="h-full w-full object-cover">
              </div>
              <div class="flex-1">
                <div class="font-bold text-slate-900">{{ product.name }}</div>
                <div class="text-orange-600 font-bold text-sm">${{ product.price.toFixed(2) }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Receipt Modal -->
    <div v-if="showReceiptModal && selectedBill" class="fixed inset-0 z-[70] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="showReceiptModal = false"></div>
      <div class="bg-white rounded-[2rem] shadow-2xl w-full max-w-sm relative z-10 overflow-hidden animate-in zoom-in duration-300">
        <div class="p-8 font-mono text-sm text-slate-800">
          <div class="text-center border-b border-dashed border-slate-200 pb-6 mb-6">
            <h1 class="text-xl font-black uppercase tracking-widest text-orange-600">Restro App</h1>
            <p class="text-[10px] text-slate-400 mt-1 uppercase">Tax Invoice / Receipt</p>
          </div>
          
          <div class="flex justify-between mb-4">
             <span class="text-slate-400">Date:</span>
             <span>{{ formatDateTime(new Date()) }}</span>
          </div>
          <div class="flex justify-between mb-8">
             <span class="text-slate-400">Table:</span>
             <span class="font-black">#{{ selectedBill.table?.tableNumber }}</span>
          </div>

          <table class="w-full mb-8">
             <thead>
               <tr class="border-b border-slate-100 text-[10px] text-slate-400 uppercase tracking-widest">
                 <th class="text-left py-2 font-medium">Item</th>
                 <th class="text-right py-2 font-medium">Price</th>
               </tr>
             </thead>
             <tbody>
               <tr v-for="item in selectedBill.orderItems" :key="item.id" class="border-b border-slate-50">
                 <td class="py-3 pr-2">
                    <div class="font-bold">{{ item.quantity }}x {{ item.product?.name }}</div>
                 </td>
                 <td class="text-right py-3 font-bold">${{ (item.quantity * item.unitPrice).toFixed(2) }}</td>
               </tr>
             </tbody>
          </table>

          <div class="space-y-2 mb-8 text-slate-500">
             <div class="flex justify-between">
                <span>Subtotal</span>
                <span>${{ billSubtotal.toFixed(2) }}</span>
             </div>
             <div class="flex justify-between text-red-500">
                <span>Discount ({{ selectedBill.discountPercentage }}%)</span>
                <span>-${{ billDiscountAmount.toFixed(2) }}</span>
             </div>
             <div class="flex justify-between">
                <span>VAT ({{ selectedBill.taxPercentage }}%)</span>
                <span>+${{ billTaxAmount.toFixed(2) }}</span>
             </div>
             <div class="flex justify-between text-lg font-black text-slate-900 pt-4 border-t border-slate-200">
                <span>Total</span>
                <span>${{ billTotal.toFixed(2) }}</span>
             </div>
          </div>

          <div class="text-center pt-6 border-t border-dashed border-slate-200">
             <p class="text-[10px] text-slate-400 uppercase tracking-[0.3em]">Thank You!</p>
             <button @click="showReceiptModal = false" class="mt-8 w-full bg-slate-100 hover:bg-slate-200 text-slate-800 py-3 rounded-xl transition-all font-bold font-sans">Close Preview</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, reactive } from 'vue'

const activeTab = ref('analytics')
const products = ref([])
const categories = ref([])
const tables = ref([])
const showModal = ref(false)
const showCategoryModal = ref(false)
const showTableModal = ref(false)
const categoryName = ref('')
const isEditing = ref(false)
const isEditingTable = ref(false)
const isSaving = ref(false)
const isSavingCategory = ref(false)
const isSavingTable = ref(false)
const fileInput = ref(null)

// Billing State
const billingSearch = ref('')
const selectedBill = ref(null)
const isSettling = ref(false)
const pendingOrders = ref([])
const showAddProductToBillModal = ref(false)
const productToBillSearch = ref('')
const isSyncingItems = ref(false)
const showReceiptModal = ref(false)
let syncDebounceTimeout = null

const tableForm = reactive({
  id: 0,
  tableNumber: '',
  capacity: 4,
  isOccupied: false
})

const form = reactive({
  id: 0,
  name: '',
  description: '',
  price: 0,
  imageUrl: '',
  categoryId: 1,
  isAvailable: true
})
const dashboardData = ref({
  totalRevenue: 0,
  totalOrders: 0,
  todaysSales: 0,
  todaysCount: 0,
  averageOrder: 0,
  salesByMethod: []
})
const topItems = ref([])
const recentOrders = ref([])

const statsCards = computed(() => [
  {
    label: 'Total Revenue',
    value: dashboardData.value.totalRevenue.toFixed(2),
    subtext: `From ${dashboardData.value.totalOrders} completed orders`,
    iconBg: 'bg-orange-100/10 text-orange-600',
    icon: 'CurrencyDollarIcon' // Using iconic names for visualization
  },
  {
    label: "Today's Sales",
    value: dashboardData.value.todaysSales.toFixed(2),
    subtext: `${dashboardData.value.todaysCount} orders today`,
    iconBg: 'bg-orange-100/10 text-orange-600',
    icon: 'TrendingUpIcon'
  },
  {
    label: 'Average Order',
    value: dashboardData.value.averageOrder.toFixed(2),
    subtext: `${dashboardData.value.totalOrders} total orders`,
    iconBg: 'bg-slate-900 text-white',
    icon: 'ShoppingBagIcon'
  }
])

const filteredOrders = computed(() => {
  if (!billingSearch.value) return pendingOrders.value
  return pendingOrders.value.filter(o => 
    o.table?.tableNumber?.toLowerCase().includes(billingSearch.value.toLowerCase()) ||
    o.id.toString().includes(billingSearch.value)
  )
})

const filteredProductsToBill = computed(() => {
  if (!productToBillSearch.value) return products.value
  const q = productToBillSearch.value.toLowerCase()
  return products.value.filter(p => p.name.toLowerCase().includes(q))
})

const billSubtotal = computed(() => {
  if (!selectedBill.value) return 0
  return selectedBill.value.orderItems.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0)
})

const billDiscountAmount = computed(() => {
  if (!selectedBill.value) return 0
  return (billSubtotal.value * (selectedBill.value.discountPercentage || 0)) / 100
})

const billTaxAmount = computed(() => {
  if (!selectedBill.value) return 0
  const taxableAmount = billSubtotal.value - billDiscountAmount.value
  return (taxableAmount * (selectedBill.value.taxPercentage || 0)) / 100
})

const billTotal = computed(() => {
  return billSubtotal.value - billDiscountAmount.value + billTaxAmount.value
})

const fetchData = async () => {
  try {
    const t = Date.now()
    const [statsRes, topRes, recentRes, productsRes, categoriesRes, tablesRes] = await Promise.all([
      fetch(`/api/dashboard/stats?t=${t}`).catch(() => ({ ok: false })),
      fetch(`/api/dashboard/top-items?t=${t}`).catch(() => ({ ok: false })),
      fetch(`/api/dashboard/recent-orders?t=${t}`).catch(() => ({ ok: false })),
      fetch(`/api/products?t=${t}`).catch(() => ({ ok: false })),
      fetch(`/api/categories?t=${t}`).catch(() => ({ ok: false })),
      fetch(`/api/tables?t=${t}`).catch(() => ({ ok: false }))
    ])

    if (statsRes.ok) dashboardData.value = await statsRes.json()
    if (topRes.ok) topItems.value = await topRes.json()
    if (recentRes.ok) recentOrders.value = await recentRes.json()
    if (productsRes.ok) products.value = await productsRes.json()
    if (tablesRes.ok) tables.value = await tablesRes.json()
    
    await fetchBillingData()

    if (categoriesRes.ok) {
        categories.value = await categoriesRes.json()
    } else {
        categories.value = [
            { id: 1, name: 'Main Course' },
            { id: 2, name: 'Appetizer' },
            { id: 3, name: 'Dessert' },
            { id: 4, name: 'Beverage' }
        ]
    }
    
    if (!statsRes.ok && !productsRes.ok) {
        console.warn('Backend seems unavailable or empty, using mock data')
        mockData()
    }
    
    console.log('Data fetch complete:', { 
      products: products.value.length, 
      tables: tables.value.length,
      orders: pendingOrders.value.length
    })
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
  }
}

const mockData = () => {
  dashboardData.value = {
    totalRevenue: 27150.45,
    totalOrders: 154,
    todaysSales: 1250.20,
    todaysCount: 12,
    averageOrder: 176.30
  }
  topItems.value = [
    { name: 'Caesar Salad', sold: 45, revenue: 404.55 },
    { name: 'Grilled Salmon', sold: 38, revenue: 721.62 },
    { name: 'Margherita Pizza', sold: 32, revenue: 415.68 }
  ]
  recentOrders.value = [
    { id: 1, tableName: 'Table 345', orderDate: new Date().toISOString(), totalAmount: 27.98, status: 'Completed', itemCount: 2 },
    { id: 2, tableName: 'Table 102', orderDate: new Date().toISOString(), totalAmount: 45.50, status: 'Paid', itemCount: 4 }
  ]
}

const fetchBillingData = async () => {
  try {
    const res = await fetch(`/api/orders/pending?t=${Date.now()}`)
    if (res.ok) {
      const data = await res.json()
      pendingOrders.value = data
      
      // If a bill is selected, update its items live too (skip if we are currently syncing custom changes)
      if (selectedBill.value && !isSyncingItems.value) {
        const updated = data.find(o => o.id === selectedBill.value.id)
        if (updated) {
          selectedBill.value.orderItems = updated.orderItems
          selectedBill.value.totalAmount = updated.totalAmount
        } else {
          // Order was deleted or settled by someone else
          selectedBill.value = null
        }
      }
    }
    
    // Also refresh tables in the background if we are on the tables tab
    if (activeTab.value === 'tables') {
      const tRes = await fetch(`/api/tables?t=${Date.now()}`)
      if (tRes.ok) tables.value = await tRes.json()
    }
  } catch (error) {
    console.error('Error fetching pending orders:', error)
  }
}

let billingInterval = null
onMounted(() => {
  fetchData()
  // Start polling for real-time order updates (every 3 seconds)
  billingInterval = setInterval(fetchBillingData, 3000)
})

onUnmounted(() => {
  if (billingInterval) clearInterval(billingInterval)
})

const selectBill = (order) => {
  // Deep clone to avoid direct mutations that skip reactivity
  selectedBill.value = JSON.parse(JSON.stringify(order))
}

const settleBill = async () => {
  if (!selectedBill.value) return
  isSettling.value = true
  try {
    const paymentMethodMap = { 'Cash': 0, 'Card': 1, 'QR': 2 }
    const response = await fetch(`/api/orders/${selectedBill.value.id}/billing`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        discountPercentage: selectedBill.value.discountPercentage,
        taxPercentage: selectedBill.value.taxPercentage,
        totalAmount: billTotal.value,
        paid: true,
        paymentMethod: paymentMethodMap[selectedBill.value.paymentMethod] || 0
      })
    })

    if (response.ok) {
      alert('Order settled and paid!')
      selectedBill.value = null
      await fetchBillingData()
      await fetchData()
    } else {
      alert('Failed to settle order')
    }
  } catch (error) {
    console.error('Settle error:', error)
  } finally {
    isSettling.value = false
  }
}

const openAddProductToBill = () => {
  productToBillSearch.value = ''
  showAddProductToBillModal.value = true
}

const addItemToBill = async (product) => {
  if (!selectedBill.value) return
  
  // Find if item already exists
  const existingItem = selectedBill.value.orderItems.find(i => i.productId === product.id)
  if (existingItem) {
    existingItem.quantity++
  } else {
    selectedBill.value.orderItems.push({
      productId: product.id,
      product: product,
      quantity: 1,
      unitPrice: product.price
    })
  }
  
  // Debounce sync with backend to batch multiple rapid clicks
  if (syncDebounceTimeout) clearTimeout(syncDebounceTimeout)
  isSyncingItems.value = true // Set immediately to block polling overwrites
  syncDebounceTimeout = setTimeout(syncOrderItems, 500)
}

const removeItemFromBill = (item) => {
  if (!selectedBill.value) return
  isSyncingItems.value = true
  
  selectedBill.value.orderItems = selectedBill.value.orderItems.filter(i => 
    (i.productId || i.id) !== (item.productId || item.id)
  )
  
  if (syncDebounceTimeout) clearTimeout(syncDebounceTimeout)
  syncDebounceTimeout = setTimeout(syncOrderItems, 500)
}

const updateItemQty = (item, delta) => {
  if (!selectedBill.value) return
  isSyncingItems.value = true
  
  const targetItem = selectedBill.value.orderItems.find(i => 
    (i.productId || i.id) === (item.productId || item.id)
  )
  
  if (targetItem) {
    targetItem.quantity += delta
    if (targetItem.quantity <= 0) {
      selectedBill.value.orderItems = selectedBill.value.orderItems.filter(i => 
        (i.productId || i.id) !== (item.productId || item.id)
      )
    }
  }
  
  if (syncDebounceTimeout) clearTimeout(syncDebounceTimeout)
  syncDebounceTimeout = setTimeout(syncOrderItems, 500)
}

const syncOrderItems = async () => {
  if (!selectedBill.value) {
    isSyncingItems.value = false
    return
  }
  
  try {
    const response = await fetch(`/api/orders/${selectedBill.value.id}/items`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(selectedBill.value.orderItems.map(i => ({
        productId: i.productId || i.id,
        quantity: i.quantity,
        unitPrice: i.unitPrice || i.price
      })))
    })
    
    if (response.ok) {
      const updatedOrder = await response.json()
      // Directly update selectedBill with fresh data from the server
      selectedBill.value = JSON.parse(JSON.stringify(updatedOrder))
      
      // Also update pendingOrders list so the sidebar is consistent
      const index = pendingOrders.value.findIndex(o => o.id === updatedOrder.id)
      if (index !== -1) {
          pendingOrders.value[index] = updatedOrder
      }
    } else {
      console.error('Failed to sync order items')
    }
  } catch (e) {
    console.error('Sync error:', e)
  } finally {
    isSyncingItems.value = false
  }
}

const formatDateShort = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' })
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return `${date.toLocaleDateString()} • ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`
}

onMounted(fetchData)

const openAddProduct = () => {
  isEditing.value = false
  Object.assign(form, { id: 0, name: '', description: '', price: 0, imageUrl: '', categoryId: categories.value[0]?.id || 1, isAvailable: true })
  showModal.value = true
}

const editProduct = (product) => {
  isEditing.value = true
  Object.assign(form, product)
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveProduct = async () => {
  isSaving.value = true
  try {
    const method = isEditing.value ? 'PUT' : 'POST'
    const url = isEditing.value ? `/api/products/${form.id}` : '/api/products'
    
    const response = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form)
    })

    if (response.ok) {
      await fetchData()
      closeModal()
      alert('Product saved successfully!')
    } else {
      const errorText = await response.text()
      alert(`Failed to save product: ${errorText}`)
    }
  } catch (error) {
    console.error('Save error:', error)
    alert('An error occurred while saving the product')
  } finally {
    isSaving.value = false
  }
}

const openAddCategory = () => {
  categoryName.value = ''
  showCategoryModal.value = true
}

const closeCategoryModal = () => {
  showCategoryModal.value = false
}

const saveCategory = async () => {
  isSavingCategory.value = true
  try {
    const response = await fetch('/api/categories', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name: categoryName.value })
    })
    if (response.ok) {
      await fetchData()
      closeCategoryModal()
    } else {
      const errorText = await response.text()
      alert(`Failed to save category: ${errorText}`)
    }
  } catch (error) {
    console.error('Error saving category:', error)
    alert('An error occurred while saving the category')
  } finally {
    isSavingCategory.value = false
  }
}

const openAddTable = () => {
  isEditingTable.value = false
  Object.assign(tableForm, { id: 0, tableNumber: '', capacity: 4, isOccupied: false })
  showTableModal.value = true
}

const editTable = (table) => {
  isEditingTable.value = true
  Object.assign(tableForm, table)
  showTableModal.value = true
}

const closeTableModal = () => {
  showTableModal.value = false
}

const saveTable = async () => {
  isSavingTable.value = true
  try {
    const method = isEditingTable.value ? 'PUT' : 'POST'
    const url = isEditingTable.value ? `/api/tables/${tableForm.id}` : '/api/tables'
    
    const response = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(tableForm)
    })

    if (response.ok) {
      await fetchData()
      closeTableModal()
      alert(isEditingTable.value ? 'Table updated!' : 'Table created!')
    } else {
      const errorText = await response.text()
      alert(`Failed to save table: ${errorText}`)
    }
  } catch (error) {
    console.error('Save error:', error)
    alert('An error occurred while saving the table')
  } finally {
    isSavingTable.value = false
  }
}

const confirmDeleteTable = async (table) => {
  if (confirm(`Delete Table ${table.tableNumber}?`)) {
    try {
      const response = await fetch(`/api/tables/${table.id}`, { method: 'DELETE' })
      if (response.ok) await fetchData()
    } catch (error) {
      console.error('Delete error:', error)
    }
  }
}

const handleFileUpload = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await fetch('/api/products/upload-image', {
      method: 'POST',
      body: formData
    })
    if (response.ok) {
      const data = await response.json()
      form.imageUrl = data.imageUrl
    } else {
      alert('Failed to upload image')
    }
  } catch (error) {
    console.error('Upload error:', error)
  }
}

const confirmDelete = async (product) => {
  if (confirm(`Are you sure you want to delete ${product.name}?`)) {
    try {
      const response = await fetch(`/api/products/${product.id}`, { method: 'DELETE' })
      if (response.ok) await fetchData()
    } catch (error) {
      console.error('Delete error:', error)
    }
  }
}

// Simple Icon Components to avoid extra deps for this demo
const CurrencyDollarIcon = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 14h-2v-1c-1.66 0-3-1.34-3-3h2c0 .55.45 1 1 1s1-.45 1-1-.45-1-1-1h-1c-1.66 0-3-1.34-3-3s1.34-3 3-3V7h2v1c1.66 0 3 1.34 3 3h-2c0-.55-.45-1-1-1s-1 .45-1 1 .45 1 1 1h1c1.66 0 3 1.34 3 3s-1.34 3-3 3v1z"/></svg>`
}
const TrendingUpIcon = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M16 6l2.29 2.29-4.88 4.88-4-4L2 16.59 3.41 18l6-6 4 4 6.3-6.29L22 12V6z"/></svg>`
}
const ShoppingBagIcon = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M18 6h-2c0-2.21-1.79-4-4-4S8 3.79 8 6H6c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2zm-6-2c1.1 0 2 .89 2 2H10c0-1.11.89-2 2-2zm0 12c-2.21 0-4-1.79-4-4h2c0 1.1.9 2 2 2s2-.9 2-2h2c0 2.21-1.79 4-4 4z"/></svg>`
}
const formatDateTime = (dateStr) => {
  const date = new Date(dateStr)
  return date.toLocaleString([], { dateStyle: 'short', timeStyle: 'short' })
}
</script>

<style scoped>
.animate-in {
  animation: fadeIn 0.4s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.zoom-in {
  animation: zoomIn 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

@keyframes zoomIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}
</style>
